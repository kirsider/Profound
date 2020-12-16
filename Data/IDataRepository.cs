using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Profound.Data.Models;
using Profound.Data.ViewModels;

namespace Profound.Data
{
    public interface IDataRepository
    {
        // GET Methods
        IEnumerable<Role> GetRoles();
        IEnumerable<User> GetUsers();
        IEnumerable<GetCourseViewModel> GetCourses();
        IEnumerable<GetCourseViewModel> GetUserCourses(int userId);
        IEnumerable<Category> GetCategories();

        IEnumerable<Module> GetCourseModules(int courseId);
        IEnumerable<Lesson> GetModuleLessons(int moduleId);
        IEnumerable<ComponentViewModel> GetComponents(int lessonId, int userId);
        IEnumerable<Comment> GetComponentComments(int componentId);
        IEnumerable<Category> GetCourseCategories(int courseId);

        User LoginUser(string email, string password);
        void RegisterUser(RegisterViewModel model);

        User GetUserByEmail(string email);
        int GetCoursesCompletedByUser(int courseId, int userId);

        bool IsEnrolled(int userId, int courseId);
        User GetUser(int userId);
        LessonViewModel GetLesson(int lessonId, int userId);
        GetCourseViewModel GetCourse(int courseId);
        Course GetBaseCourse(int courseId);
        int GetLastLessonId(int courseId, int userId);
        void PostLesson(PostLessonViewModel model);
        void CompleteCourse(CompleteCourseViewModel model);
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

        void RequestToPublish(int course_id);
        void PublishCourse(int course_id);
        void RejectCourse(int course_id);

        void PostEnrollment(UserCourseEnrollment enrollment);
        void PostPurchase(Payment payment);
        void UpdateLastLessonId(int lessonId, int courseId, int userId);

        void CreateCategoryCourse(int courseId, int categoryId);

        Comment PutComment(int commentId, CommentPutRequest commentPutRequest);

        // DELETE Methods
        void DeleteСourse(int course_id);

        void DeleteComment(int commentId);
    }
}
