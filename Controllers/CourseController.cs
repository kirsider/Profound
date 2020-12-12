using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Profound.Data;
using Profound.Data.Models;

namespace Profound.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;

        public CourseController(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        [HttpGet]
        public IEnumerable<Course> GetCourses()
        {
            return _dataRepository.GetCourses();
        }

        [HttpGet("{courseId}")]
        public ActionResult<Course> GetCourse(int courseId)
        {
            int userId = 5;

            // return basic course details for overview mode, extended course data - for course passing
            Course course = _dataRepository.IsEnrolled(userId, courseId) ?
                _dataRepository.GetCourse(courseId) : _dataRepository.GetBaseCourse(courseId);

            if (course == null)
            {
                return NotFound();
            }
            return course;
        }

        [HttpGet("{courseId}/modules")]
        public IEnumerable<Module> GetModules(int courseId)
        {
            return _dataRepository.GetCourseModules(courseId);
        }

        [HttpGet("{courseId}/modules/{moduleId}")]
        public IEnumerable<Lesson> GetLessons(int courseId, int moduleId)
        {
            int userId = 5;
            Course course = _dataRepository.IsEnrolled(userId, courseId) ?
                _dataRepository.GetCourse(courseId) : null;
            Module module = course != null ? course.Modules.Where(m => m.Id == moduleId).FirstOrDefault() : null;
            return module != null ? _dataRepository.GetModuleLessons(module.Id) : null;
        }

        [HttpGet("{courseId}/modules/{moduleId}/{lessonId}")]
        public IEnumerable<Component> GetComponents(int courseId, int moduleId, int lessonId)
        {
            int userId = 5;
            Course course = _dataRepository.IsEnrolled(userId, courseId) ?
                _dataRepository.GetCourse(courseId) : null;
            Module module = course != null ? course.Modules.Where(m => m.Id == moduleId).FirstOrDefault() : null;
            Lesson lesson = module != null ? module.Lessons.Where(l => l.Id == lessonId).FirstOrDefault() : null;
            return _dataRepository.GetLessonComponents(lesson.Id);
        }

        [HttpGet("component/{component_id}/comment")]
        public IEnumerable<Comment> GetComments(int component_id)
        {
            return _dataRepository.GetCommentsFromComponent(component_id);
        }

        [HttpPost("comment")]
        public ActionResult<Comment> PostComment(CommentPostRequest commentPostRequest)
        {
            var savedComment = _dataRepository.PostComment(new Comment
            {
                ComponentId = commentPostRequest.ComponentId,
                UserId = 1,
                Text = commentPostRequest.Text,
                CreatedAt = DateTime.Now
            });

            return CreatedAtAction("PostComment", savedComment);
        }

        [HttpPut("comment/{commentId}")]
        public ActionResult<Comment> PutComment(int commentId, CommentPutRequest commentPutRequest)
        {
            var comment = _dataRepository.GetComment(commentId);

            if (comment == null)
            {
                return NotFound();
            }

            commentPutRequest.Text = string.IsNullOrEmpty(commentPutRequest.Text) ? comment.Text : commentPutRequest.Text;

            var savedComment = _dataRepository.PutComment(commentId, commentPutRequest);
            return savedComment;
        }

        [HttpDelete("comment/{commentId}")]
        public ActionResult DeleteComment(int commentId)
        {
            var comment = _dataRepository.GetComment(commentId);

            if (comment == null)
            {
                return NotFound();
            }

            _dataRepository.DeleteComment(commentId);
            return NoContent();
        }

        [HttpPost("{courseId}/enroll")]
        public IActionResult PostEnrollment(int courseId)
        {
            int userId = 5;  // just dummy id till jwt introduced
            _dataRepository.PostEnrollment(new UserCourseEnrollment
            {
                CourseId = courseId,
                UserId = userId
            });

            return Ok();
        }

        [HttpPost("{courseId}/purchase")]
        public IActionResult Purchase(int courseId)
        {
            int userId = 5;
            _dataRepository.PostPurchase(new Payment { CourseId = courseId, UserId = userId });
            _dataRepository.PostEnrollment(new UserCourseEnrollment
            {
                CourseId = courseId,
                UserId = userId
            });

            return Ok();
        }
    }
}
