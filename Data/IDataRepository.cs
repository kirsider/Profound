using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Profound.Data.Models;

namespace Profound.Data
{
    public interface IDataRepository
    {
        // GET Methods

        IEnumerable<Role> GetRoles();
        IEnumerable<User> GetUsers();
        IEnumerable<Course> GetCourses();
        IEnumerable<Module> GetCourseModules(int courseId);
        IEnumerable<Lesson> GetModuleLessons(int moduleId);
        IEnumerable<Component> GetLessonComponents(int lessonId);
        IEnumerable<Comment> GetComponentComments(int componentId);

        bool GetEnrollment(int userId, int courseId);
        User GetUser(int userId);
        Course GetCourse(int courseId);
        Category GetCategory(int categoryId);
        Comment GetComment(int commentId);
        CourseStats GetCourseStats(int courseId, int offset, int limit);
        ComponentStats GetComponentStats(int componentId);


        // POST Methods

        Comment PostComment(Comment comment);

        Course CreateCourse(Course course);

        Module CreateModule(Module module);

        Lesson CreateLesson(Lesson lesson);

        Component CreateComponent(Component component);

        void RequestToModeration(int course_id);

        public Course PostEnrollment(UserCourseEnrollment enrollment);

        // PUT Methods

        Comment PutComment(int commentId, CommentPutRequest commentPutRequest);

        // DELETE Methods

        void DeleteComment(int commentId);
    }
}
