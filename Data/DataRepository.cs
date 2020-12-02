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

        public bool GetEnrollment(UserCourseEnrollment userCourseEnrollment)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var enrollment = connection.QueryFirstOrDefault<Course>(
                    @"SELECT * FROM User_course_enrollment WHERE user_id=@UserId and course_id=@CourseId",
                    new { UserId = userCourseEnrollment.UserId, CourseId = userCourseEnrollment.CourseId }
                    );
                return false ? enrollment is null : true;
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
                    Pagination = new Pagination { 
                        Total = courseStatsTotal.Count(),
                        Limit = limit, Offset = offset,
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
    }
}
