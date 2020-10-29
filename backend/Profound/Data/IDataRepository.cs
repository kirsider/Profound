using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Profound.Data.Models;

namespace Profound.Data
{
    public interface IDataRepository
    {
        IEnumerable<Role> GetRoles();
        IEnumerable<User> GetUsers();
        IEnumerable<Course> GetCourses();
        IEnumerable<Module> GetCourseModules(int courseId);
        IEnumerable<Lesson> GetModuleLessons(int moduleId);
        IEnumerable<Component> GetLessonComponents(int lessonId);
        IEnumerable<Comment> GetComponentComments(int componentId);

        User GetUser(int userId);
        Course GetCourse(int courseId);
        Category GetCategory(int categoryId);
    }
}
