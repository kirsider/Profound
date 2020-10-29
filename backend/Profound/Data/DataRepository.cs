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
    public class DataRepository: IDataRepository
    {
        private readonly string _connectionString;

        public DataRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:MySQLConnection"];
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
                    WHERE id=@UserId;", new { UserId = userId}
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
    }
}
