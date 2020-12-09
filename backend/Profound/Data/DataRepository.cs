using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Profound.Data.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace Profound.Data
{
    public class DataRepository : IDataRepository
    {
        private readonly string _connectionString;

        public DataRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:AzureConnection"];
        }


        public IEnumerable<User> GetUsers()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<User>(
                    @"SELECT id, role_id AS roleId, email, first_name AS firstName, last_name AS lastName FROM User;"
                );
            }
        }

        public User GetUser(int userId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<User>(
                    @"SELECT id, role_id AS roleId, email, first_name AS firstName, last_name AS lastName FROM User 
                    WHERE id=@UserId;", new { UserId = userId }
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

        public IEnumerable<Course> GetCourses()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<Course>(
                    @"SELECT id, creator_id AS creatorId, title, description, price, 
                        acceptance_percantage AS acceptancePercantage, requirements, 
                        status, published_at AS publishedAt FROM Course;"
                );
            }
        }

        public IEnumerable<Module> GetCourseModules(int courseId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var modules = connection.Query<Module>(
                    @"SELECT id, course_id AS courseId, name FROM Module
                      WHERE course_id=@CourseId;", new { CourseId = courseId }
                );

                foreach (var module in modules)
                {
                    module.Lessons = GetModuleLessons(module.Id);
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
                    @"SELECT id, module_id AS moduleId, name FROM Lesson
                      WHERE module_id=@ModuleId;", new { ModuleId = moduleId }
                );

                foreach (var lesson in lessons)
                {
                    lesson.Components = GetLessonComponents(lesson.Id);
                }

                return lessons;
            }
        }

        public IEnumerable<Component> GetLessonComponents(int lessonId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var components = connection.Query<Component>(
                    @"SELECT id, lesson_id AS lessonId, max_points AS maxPoints, content FROM Component
                      WHERE lesson_id=@LessonId;", new { LessonId = lessonId }
                );

                foreach (var component in components)
                {
                    component.Comments = GetComponentComments(component.Id);
                }

                return components;
            }
        }

        public IEnumerable<Comment> GetComponentComments(int componentId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<Comment>(
                    @"SELECT id, user_id AS userId, component_id AS componentId, text FROM Comment
                      WHERE component_id=@ComponentId;", new { ComponentId = componentId }
                );
            }
        }

        public Course GetCourse(int courseId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var course = connection.QueryFirstOrDefault<Course>(
                    @"SELECT id, creator_id AS creatorId, title, description, price, 
                        acceptance_percantage AS acceptancePercantage, requirements, 
                        status, published_at AS publishedAt FROM Course
                    WHERE id=@CourseId;", new { CourseId = courseId }
                );

                if (course != null)
                {
                    course.Modules = GetCourseModules(courseId);
                }

                return course;
            }
        }

        public bool GetEnrollment(int userId, int courseId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var enrollment = connection.QueryFirstOrDefault<UserCourseEnrollment>(
                    @"SELECT * FROM User_course_enrollment WHERE user_id=@UserId and course_id=@CourseId",
                    new { UserId = userId, CourseId = courseId }
                    );

                return enrollment == null ? false : true;
            }
        }
        public Category GetCategory(int categoryId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<Category>(
                    @"SELECT id, name FROM Category WHERE id=@CategoryId;", new { CategoryId = categoryId }
                );
            }
        }

        public Comment GetComment(int commentId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<Comment>(
                    @"SELECT id, user_id AS userId, component_id AS componentId, text FROM Comment WHERE id=@CommentId;",
                    new { CommentId = commentId }
                );
            }
        }

        public void RequestToModeration(int course_id)
        {
            string status = "on_moderation";
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"UPDATE Course SET status=@Status WHERE id=@CourseId;",
                    new { Status = status, CourseId = course_id}
                );                
            }
        }

        public Comment PostComment(Comment comment)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"INSERT INTO Comment( user_id, component_id, text) VALUES (@UserId, @ComponentId, @Text);",
                    new { comment.UserId, comment.ComponentId, comment.Text }
                );
                return connection.QueryFirst<Comment>(
                    @"SELECT id, user_id AS userId, component_id AS componentId, text FROM Comment ORDER BY id DESC LIMIT 1;"
                );
            }
        }

        public Course CreateCourse(Course course)
        {
            string status = "dev";
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"INSERT INTO Course(creator_id, title, description, price, acceptance_percantage, requirements,
                    status, published_at) 
                    VALUES(@CreatorId, @Title, @Description, @Price, @AcceptancePersantage, @Requirements, 
                    @Status, @PublishedAt);",
                    new
                    {                        
                        course.CreatorId,
                        course.Title,
                        course.Description,
                        course.Price,
                        course.AcceptancePercantage,
                        course.Requirements,
                        status,
                        DateTime.Now
                    }
                );
                return GetCourse(course.Id);
            }
        }

        public Module CreateModule(Module module)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"INSERT INTO Module(course_id, name) 
                    VALUES(@courseId, @name);",
                    new
                    {                        
                        module.CourseId,
                        module.Name
                    }
                );
                return GetCourseModules(module.CourseId).Where(m => m.Id == module.Id).FirstOrDefault();
            }
        }

        public Lesson CreateLesson(Lesson lesson)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"INSERT INTO Lesson(module_id, name) 
                    VALUES(@moduleId, @name);",
                    new
                    {                        
                        lesson.ModuleId,
                        lesson.Name
                    }
                );
                return GetModuleLessons(lesson.ModuleId).Where(l => l.Id == lesson.Id).FirstOrDefault();
            }
        }

        public Component CreateComponent(Component component)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"INSERT INTO Lesson(lesson_id, max_points, content) 
                    VALUES(@lessonId, @maxPoints, @content);",
                    new
                    {                        
                        component.LessonId,
                        component.MaxPoints,
                        component.Content
                    }
                );
                return GetLessonComponents(component.LessonId).Where(c => c.Id == component.Id).FirstOrDefault();
            }
        }

        public Course PostEnrollment(UserCourseEnrollment enrollment)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"INSERT INTO User_course_enrollment(user_id, course_id, status) 
                    VALUES(@UserId, @CourseId, @Status);",
                    new { enrollment.UserId, enrollment.CourseId, enrollment.Status }
                );
                return connection.QueryFirst<Course>(
                    @"SELECT title From Course where id=@Id", new { Id = enrollment.CourseId }
                );
            }
        }

        public Comment PutComment(int commentId, CommentPutRequest commentPutRequest)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    @"UPDATE Comment SET text = @Text WHERE id = @Id;",
                    new { commentPutRequest.Text, Id = commentId }
                );
                return GetComment(commentId);
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
    }
}
