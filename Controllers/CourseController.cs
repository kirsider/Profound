using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Profound.Data;
using Profound.Data.Models;
using Profound.Data.ViewModels;

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
        public IEnumerable<GetCourseViewModel> GetCourses(int categoryId)
        {
            return _dataRepository.GetCourses();
        }

        [HttpGet("user")]
        public ActionResult<IEnumerable<GetCourseViewModel>> GetUserCourses()
        {
            var identity = User.Identity;
            if (identity.Name == null)
            {
                return Unauthorized("Error 401. Please, authorize first to get access to this resource.");
            }

            var userId = _dataRepository.GetUserByEmail(identity.Name).Id;
            return Ok(_dataRepository.GetUserCourses(userId));
        }

        [HttpGet("{courseId}")]
        public ActionResult<UserCourseViewModel> GetCourse(int courseId)
        {
            GetCourseViewModel course = _dataRepository.GetCourse(courseId);
            if (course == null)
            {
                return NotFound();
            }

            var courseCategories = _dataRepository.GetCourseCategories(courseId);
            var identity = User.Identity;

            int lastLessonId = 0;
            var user = _dataRepository.GetUserByEmail(identity.Name);
            if (user != null)
            {
                lastLessonId = _dataRepository.GetLastLessonId(courseId, user.Id);
            }

            return Ok(new UserCourseViewModel
            {
                Course = course,
                CourseCategories = courseCategories.ToList(),
                LastLessonId = lastLessonId
            });
        }

        [HttpGet("{courseId}/lesson/{lessonId}")]
        public ActionResult<Lesson> GetLesson(int courseId, int lessonId)
        {
            var identity = User.Identity;
            if (identity.Name == null)
            {
                return Unauthorized("Error 401. Please, authorize first to get access to this resource.");
            }

            var userId = _dataRepository.GetUserByEmail(identity.Name).Id;

            Course course = _dataRepository.GetBaseCourse(courseId);
            if (course == null)
            {
                return NotFound();
            }

            LessonViewModel lesson = _dataRepository.GetLesson(lessonId, userId);

            if (lesson == null)
            {
                return NotFound();
            }
            _dataRepository.UpdateLastLessonId(lessonId, courseId, userId);

            return Ok(lesson);
        }

        [HttpPost("lessons")]
        public ActionResult PostLesson(PostLessonViewModel model)
        {
            _dataRepository.PostLesson(model);
            return Ok();
        }

        [HttpPost("completeCourse")]
        public ActionResult CompleteCourse(CompleteCourseViewModel model)
        {
            _dataRepository.CompleteCourse(model);
            return Ok();
        }

        [HttpPost("comments")]
        public ActionResult<Comment> PostComment(CommentPostRequest commentPostRequest)
        {
            var identity = User.Identity;
            if (identity.Name == null)
            {
                return Unauthorized("Error 401. Please, authorize first to get access to this resource.");
            }

            var userId = _dataRepository.GetUserByEmail(identity.Name).Id;

            var savedComment = _dataRepository.PostComment(new Comment
            {
                ComponentId = commentPostRequest.ComponentId,
                UserId = userId,
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
            var identity = User.Identity;
            if (identity.Name == null)
            {
                return Unauthorized("Error 401. Please, authorize first to get access to this resource.");
            }

            var userId = _dataRepository.GetUserByEmail(identity.Name).Id;

            Course course = _dataRepository.GetBaseCourse(courseId);
            if (course == null)
            {
                return NotFound();
            }
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
            var identity = User.Identity;
            if (identity.Name == null)
            {
                return Unauthorized("Error 401. Please, authorize first to get access to this resource.");
            }

            var userId = _dataRepository.GetUserByEmail(identity.Name).Id;

            _dataRepository.PostPurchase(new Payment { CourseId = courseId, UserId = userId });
            _dataRepository.PostEnrollment(new UserCourseEnrollment
            {
                CourseId = courseId,
                UserId = userId
            });

            return Ok();
        }

        [HttpGet("categories")]
        public IEnumerable<Category> GetCategories()
        {
            return _dataRepository.GetCategories();
        }

    }
}
