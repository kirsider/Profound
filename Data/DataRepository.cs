using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Profound.Data.Models;
using Dapper;
using MySql.Data.MySqlClient;
using Profound.Data.ViewModels;

namespace Profound.Data
{
    public class DataRepository : IDataRepository
    {
        private readonly string _connectionString;

        public DataRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:AzureConnection"];
        }

        public User LoginUser(string email, string password)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var user = connection.QueryFirstOrDefault<User>(@"SELECT id, role_id as roleId, email, last_name as " +
                    "lastName, first_name as firstName FROM user WHERE email=@Email and password_hash = MD5(@Password);",
                    new { Email = email, Password = password });
                if (user != null)
                {
                    user.Role = GetRoles().Where(r => r.Id == user.RoleId).FirstOrDefault();
                }
                return user;
            }
        }

        public void RegisterUser(RegisterViewModel model)
        {
            int UserRole = 1;
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(@"INSERT INTO user(role_id, first_name, last_name, email, password_hash) 
                VALUES (@UserRole, @FirstName, @LastName, @Email, MD5(@Password));",
                new { UserRole, model.FirstName, model.LastName, model.Email, model.Password });
            }
        }

        public User GetUserByEmail(string email)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                return connection.QueryFirstOrDefault<User>(
                    @"SELECT id, role_id AS roleId, email, first_name AS firstName, last_name AS lastName 
                        FROM user WHERE email=@Email;",
                    new { Email = email });
            }
        }

        public IEnumerable<User> GetUsers()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                IEnumerable<User> users = connection.Query<User>(
                    @"SELECT id, role_id AS roleId, email, first_name AS firstName, last_name AS lastName FROM User;"
                );
                foreach (var user in users)
                {
                    user.Role = GetRoles().Where(r => r.Id == user.RoleId).FirstOrDefault();
                }
                return users;
            }
        }

        public User GetUser(int userId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<User>(
                    @"SELECT id, role_id AS roleId, `email`, first_name AS firstName, last_name AS lastName FROM User 
                    WHERE id=@UserId;", new { UserId = userId }
                );
            }
        }

        public int GetCoursesCompletedByUser(int courseId, int userId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                return Convert.ToInt32(connection.ExecuteScalar<int>(
                    @"SELECT COUNT(*) FROM User_Course_Enrollment 
                        WHERE course_id=@CourseId AND user_id=@UserId AND status='completed';",
                    new { CourseId = courseId, UserId = userId}));
            }
        }

        public int GetLastLessonId(int courseId, int userId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<int>(
                    @"SELECT last_lesson_id  FROM User_course_enrollment WHERE user_id=@UserId and course_id=@CourseId",
                    new { UserId = userId, CourseId = courseId }
                );
            }
        }

        public IEnumerable<Role> GetRoles()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<Role>(
                    @"SELECT id, role as roleName FROM Role;"
                );
            }
        }

        public IEnumerable<GetCourseViewModel> GetCourses()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var courses = connection.Query<Course>(
                    @"SELECT id, creator_id AS creatorId, `title`, description, price, 
                        acceptance_percantage AS acceptancePercantage, requirements, 
                        `status`, published_at AS publishedAt FROM Course;"
                );

                if (courses == null) return null;
                List<GetCourseViewModel> courseVMs = new List<GetCourseViewModel>();
                
                foreach (var course in courses)
                {
                    var creator = GetUser(course.CreatorId);

                    courseVMs.Add(new GetCourseViewModel
                    {
                        Id = course.Id,
                        Creator = new CourseUserViewModel
                        {
                            Id = creator.Id,
                            FirstName = creator.FirstName,
                            LastName = creator.LastName
                        },
                        Title = course.Title,
                        Description = course.Description,
                        Price = course.Price,
                        AcceptancePercantage = course.AcceptancePercantage,
                        Status = course.Status,
                        Requirements = course.Requirements,
                        PublishedAt = course.PublishedAt
                    });
                }
                return courseVMs;
            }
        }

        public IEnumerable<GetCourseViewModel> GetUserCourses(int userId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var courses = connection.Query<Course>(
                    @"SELECT c.id, creator_id AS creatorId, `title`, description, price, 
                        acceptance_percantage AS acceptancePercantage, requirements, 
                        c.`status`, published_at AS publishedAt FROM course AS c
                         JOIN user_course_enrollment AS uc ON(c.id = uc.course_id)
                         WHERE uc.user_id = @UserId;", new { UserId = userId}
                );

                if (courses == null) return null;
                List<GetCourseViewModel> courseVMs = new List<GetCourseViewModel>();

                foreach (var course in courses)
                {
                    var creator = GetUser(course.CreatorId);

                    courseVMs.Add(new GetCourseViewModel
                    {
                        Id = course.Id,
                        Creator = new CourseUserViewModel
                        {
                            Id = creator.Id,
                            FirstName = creator.FirstName,
                            LastName = creator.LastName
                        },
                        Title = course.Title,
                        Description = course.Description,
                        Price = course.Price,
                        AcceptancePercantage = course.AcceptancePercantage,
                        Status = course.Status,
                        Requirements = course.Requirements,
                        PublishedAt = course.PublishedAt
                    });
                }
                return courseVMs;
            }

        }

        public IEnumerable<Category> GetCourseCategories(int courseId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var categories = connection.Query<Category>(
                    @"SELECT c.id, name FROM Category_Course cc 
                        JOIN Category c ON cc.category_id = c.id
                        WHERE cc.course_id = @CourseId;",
                    new { CourseId = courseId }
                );

                return categories;
            }
        }

        public IEnumerable<Module> GetCourseModules(int courseId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var modules = connection.Query<Module>(
                    @"SELECT id, course_id AS courseId, `name`, `order` 
                        FROM Module WHERE course_id = @CourseId ORDER BY `order`;", new { CourseId = courseId }
                );

                if (modules != null)
                {
                    foreach (var module in modules)
                    {
                        module.Lessons = GetModuleLessons(module.Id);
                    }
                }

                return modules;
            }
        }

        public IEnumerable<Lesson> GetModuleLessons(int moduleId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var lessons = connection.Query<Lesson>(
                    @"SELECT id, module_id AS moduleId, `name`, `order` FROM Lesson
                      WHERE module_id=@ModuleId ORDER BY `order`;", new { ModuleId = moduleId }
                );

                return lessons;
            }
        }


        public IEnumerable<ComponentViewModel> GetComponents(int lessonId, int userId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var componentVMs = connection.Query<ComponentViewModel>(
                  @"SELECT c.id, lesson_id AS lessonId, max_points AS maxPoints, component_type AS componentType, 
                      `content`, `order`, (CASE WHEN status IN ('correct', 'wrong') THEN 1 ELSE 0 END) AS completed
                      FROM User_solution AS us JOIN Component AS c ON us.component_id = c.id
                      WHERE c.lesson_id=@LessonId AND us.user_id = @UserId ORDER BY `order`;",
                  new { LessonId = lessonId, UserId = userId }
                );

                foreach (var componentVM in componentVMs)
                {
                    componentVM.Comments = GetComponentComments(componentVM.Id);
                }
                return componentVMs;
            }
        }


        public LessonViewModel GetLesson(int lessonId, int userId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var lesson = connection.QueryFirstOrDefault<Lesson>(
                    @"SELECT id, module_id AS moduleId, `name`, `order` FROM Lesson
                      WHERE id=@LessonId ORDER BY `order`;", new { LessonId = lessonId }
                );
                LessonViewModel lessonVM = null;


                if (lesson != null)
                {
                    lessonVM = new LessonViewModel
                    {
                        Id = lesson.Id,
                        ModuleId = lesson.ModuleId,
                        Name = lesson.Name,
                        Order = lesson.Order
                    };
                    lessonVM.Components = GetComponents(lessonId, userId);
                }

                return lessonVM;
            }
        }

        public IEnumerable<Comment> GetComponentComments(int componentId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<Comment>(
                    @"SELECT id, user_id AS userId, component_id AS componentId, `text`, created_at AS createdAt
                    FROM Comment WHERE component_id=@ComponentId;", new { ComponentId = componentId }
                );
            }
        }
        public Course GetBaseCourse(int courseId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var course = connection.QueryFirstOrDefault<Course>(
                    @"SELECT id, creator_id AS creatorId, `title`, description, `price`, 
                        acceptance_percantage AS acceptancePercantage, requirements, 
                        `status`, published_at AS publishedAt FROM Course
                    WHERE id=@CourseId;", new { CourseId = courseId }
                );

                return course;
            }
        }

        public GetCourseViewModel GetCourse(int courseId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var baseCourse = GetBaseCourse(courseId);

                if (baseCourse == null) return null;
                
                var creator = GetUser(baseCourse.CreatorId);
                return new GetCourseViewModel
                {
                    Id = baseCourse.Id,
                    Creator = new CourseUserViewModel
                    {
                        Id = creator.Id,
                        FirstName = creator.FirstName,
                        LastName = creator.LastName
                    },
                    Title = baseCourse.Title,
                    Description = baseCourse.Description,
                    Price = baseCourse.Price,
                    AcceptancePercantage = baseCourse.AcceptancePercantage,
                    Status = baseCourse.Status,
                    Requirements = baseCourse.Requirements,
                    PublishedAt = baseCourse.PublishedAt,
                    Modules = GetCourseModules(courseId)
                };
            }
        }


        public bool IsEnrolled(int userId, int courseId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var enrollment = connection.QueryFirstOrDefault<UserCourseEnrollment>(
                    @"SELECT * FROM User_course_enrollment WHERE user_id=@UserId and course_id=@CourseId",
                    new { UserId = userId, CourseId = courseId }
                    );

                return enrollment != null;
            }
        }
        public Category GetCategory(int categoryId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<Category>(
                    @"SELECT id, `name` FROM Category WHERE id=@CategoryId;", new { CategoryId = categoryId }
                );
            }
        }
        public IEnumerable<Category> GetCategories()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<Category>(
                    @"SELECT id, `name` FROM Category");
            }
        }

        public Comment GetComment(int commentId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<Comment>(
                    @"SELECT id, user_id AS userId, component_id AS componentId, `text`, created_at AS createdAt 
                        FROM Comment WHERE id=@CommentId;",
                    new { CommentId = commentId }
                );
            }
        }

        public void ChangeCourseStatus(string status, int course_id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"UPDATE Course SET status=@Status WHERE id=@CourseId;",
                    new { Status = status, CourseId = course_id }
                );
            }
        }

        public void RejectCourse(int course_id)
        {
            ChangeCourseStatus("dev", course_id);
        }

        public void RequestToPublish(int course_id)
        {
            ChangeCourseStatus("on_moderation", course_id);
        }

        public void PublishCourse(int courseId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"UPDATE Course SET published_at=@PublishedAt WHERE id=@CourseId;",
                    new { PublishedAt = DateTime.Now, CourseId = courseId }
                );
            }
            ChangeCourseStatus("published", courseId);
        }

        public Comment PostComment(Comment comment)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"INSERT INTO Comment( user_id, component_id, `text`, created_at) 
                      VALUES (@UserId, @ComponentId, @Text, @CreatedAt);",
                    new { comment.UserId, comment.ComponentId, comment.Text, comment.CreatedAt }
                );
                return connection.QueryFirst<Comment>(
                    @"SELECT id, user_id AS userId, component_id AS componentId, `text`, created_at AS createdAt 
                      FROM Comment ORDER BY id DESC LIMIT 1;"
                );
            }
        }

        public void CreateCategoryCourse(int courseId, int categoryId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"INSERT INTO category_course(course_id, category_id) 
                      VALUES (@CourseId, @CategoryId);",
                    new { CourseId = courseId, CategoryId = categoryId }
                );
            }
        }

        public Course CreateCourse(Course course)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var courseId = Convert.ToInt32(connection.ExecuteScalar(
                    @"INSERT INTO Course(creator_id, `title`, description, `price`, acceptance_percantage, requirements,
                        `status`, published_at) 
                        VALUES(@CreatorId, @Title, @Description, @Price, @AcceptancePercantage, @Requirements, 
                        @Status, @PublishedAt);
                        SELECT LAST_INSERT_ID();",
                    new
                    {
                        CreatorId = course.CreatorId,
                        Title = course.Title,
                        Description = course.Description,
                        Price = course.Price,
                        AcceptancePercantage = course.AcceptancePercantage,
                        Requirements = course.Requirements,
                        Status = course.Status,
                        PublishedAt = DateTime.Now
                    }
                ));
                course.Id = courseId;
                return course;
            }
        }

        public Module CreateModule(Module module)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var moduleId = Convert.ToInt32(connection.ExecuteScalar(
                    @"INSERT INTO Module(course_id, `name`, `order`) 
                        VALUES(@CourseId, @Name, @Order);
                        SELECT LAST_INSERT_ID();",
                    new
                    {
                        CourseId = module.CourseId,
                        Name = module.Name,
                        Order = module.Order
                    }
                ));
                module.Id = moduleId;
                foreach (var lesson in module.Lessons)
                {
                    lesson.ModuleId = module.Id;
                    var createdLesson = CreateLesson(lesson);
                    lesson.Id = createdLesson.Id;
                }

                return module;
            }
        }

        public Lesson CreateLesson(Lesson lesson)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var lessonId = Convert.ToInt32(connection.ExecuteScalar(
                    @"INSERT INTO Lesson(module_id, `name`, `order`) 
                        VALUES(@ModuleId, @Name, @Order);
                        SELECT LAST_INSERT_ID();",
                    new
                    {
                        ModuleId = lesson.ModuleId,
                        Name = lesson.Name,
                        Order = lesson.Order
                    }
                ));

                lesson.Id = lessonId;
                foreach (var component in lesson.Components)
                {
                    component.LessonId = lesson.Id;
                    var createdComponent = CreateComponent(component);
                    component.Id = createdComponent.Id;
                }

                return lesson;
            }
        }

        public Component CreateComponent(Component component)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var componentId = Convert.ToInt32(connection.ExecuteScalar(
                    @"INSERT INTO Component(lesson_id, max_points, component_type, `content`, `order`) 
                        VALUES(@LessonId, @MaxPoints, @ComponentType, @Content, @Order);
                        SELECT LAST_INSERT_ID();",
                    new
                    {
                        LessonId = component.LessonId,
                        MaxPoints = component.MaxPoints,
                        ComponentType = component.ComponentType,
                        Content = component.Content,
                        Order = component.Order
                    }
                ));

                component.Id = componentId;
                return component;
            }
        }

        public void PostEnrollment(UserCourseEnrollment enrollment)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"INSERT INTO User_course_enrollment(user_id, course_id, `status`) 
                    VALUES(@UserId, @CourseId, IFNULL(@Status, 'in_process'));",
                    new { enrollment.UserId, enrollment.CourseId, enrollment.Status }
                );
            }
        }

        public void PostLesson(PostLessonViewModel model)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                int lessonPoints = 0;
                int userId = 0;
                connection.Open();
                foreach (var solution in model.Solutions)
                {
                    connection.Execute(
                        @"INSERT INTO user_solution(component_id, user_id, `status`, points, answer)
                                VALUES(@ComponentId, @UserId, @Status, @Points, @Answer);",
                        new { solution.ComponentId, solution.UserId, solution.Status, solution.Points, solution.Answer }
                    );
                    userId = solution.UserId;
                    lessonPoints += solution.Points;
                }
                connection.Execute(
                        @"UPDATE user_course_enrollment SET last_lesson_id=@LastLessonId, total_points=
                            total_points+@LessonPoints WHERE user_id=@UserId and course_id=@CourseId;",
                        new { LastLessonId = model.LessonId, lessonPoints, userId, model.CourseId }
                    );
            }
        }

        public void CompleteCourse(CompleteCourseViewModel model)
        {
            string Completed = "completed";
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"UPDATE user_course_enrollment SET `status`=@Completed WHERE course_id=@CourseId 
                        and user_id=@UserId;",
                    new { Completed, model.CourseId, model.UserId }
                );
            }
        }

        public Comment PutComment(int commentId, CommentPutRequest commentPutRequest)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"UPDATE Comment SET `text` = @Text WHERE id = @Id;",
                    new { commentPutRequest.Text, Id = commentId }
                );
                return GetComment(commentId);
            }
        }

        public void DeleteСourse(int course_id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"DELETE FROM course WHERE id = @Id;",
                    new { Id = course_id }
                );
            }
        }

        public void DeleteComment(int commentId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"DELETE FROM Comment WHERE id = @Id;",
                    new { Id = commentId }
                );
            }
        }

        public CourseStats GetCourseStats(int courseId, int offset, int limit)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var id = connection.QueryFirstOrDefault<int>(
                   @"SELECT id FROM Course WHERE id=@CourseId;", new { CourseId = courseId }
                 );

                if (id == 0) return null;

                var courseStatsTotal = connection.Query<UserCoursePoints>(
                    @"SELECT first_name AS firstName, last_name AS lastName, SUM(us.points) AS totalPoints
                      FROM (SELECT user_id FROM User_course_enrollment WHERE course_id=@CourseId) AS cu
				      JOIN User_solution AS us ON cu.user_id = us.user_id
					  JOIN `User` AS u ON cu.user_id = u.id
                      GROUP BY us.user_id, first_name, last_name
                      ORDER BY SUM(us.points) DESC;", new { CourseId = courseId }
                );

                var statsPage = courseStatsTotal.Skip(offset).Take(limit);
                return new CourseStats
                {
                    UsersCourseStats = statsPage,
                    Pagination = new Pagination
                    {
                        Total = courseStatsTotal.Count(),
                        Limit = limit,
                        Offset = offset,
                        Returned = statsPage.Count()
                    }
                };

            }
        }

        public ComponentStats GetComponentStats(int componentId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var id = connection.QueryFirstOrDefault<int>(
                    @"SELECT id FROM Component WHERE id=@ComponentId;", new { ComponentId = componentId }
                );

                if (id == 0) return null;

                return connection.QueryFirstOrDefault<ComponentStats>(
                    @"SELECT 
                    SUM(CASE WHEN `Status` = 'correct' THEN 1 ELSE 0 END) AS answeredCorrectly,
                    SUM(CASE WHEN `Status` != 'uncompleted' THEN 1 ELSE 0 END) AS participantsTotal 
                    FROM User_solution AS us
                    JOIN Component AS c ON us.Component_id = c.id  
                    WHERE Component_id = @ComponentId AND Component_type = 'practice'; ", new { ComponentId = componentId }
                );
            }
        }

        public void PostPurchase(Payment payment)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"INSERT INTO Payment(course_id, user_id) VALUES (@CourseId, @UserId);",
                    new { CourseId = payment.CourseId, UserId = payment.UserId }
                );
            }
        }

        public void UpdateLastLessonId(int lessonId, int courseId, int userId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"UPDATE User_course_enrollment SET last_lesson_id = @LessonId WHERE course_id = @CourseId AND user_id = @UserId;",
                    new { LessonId = lessonId, CourseId = courseId, UserId = userId }
                );
            }
        }
    }
}
