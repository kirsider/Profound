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
        public IEnumerable<Course> GetCourses()
        {
            return _dataRepository.GetCourses();
        }

        [HttpGet("{courseId}")]
        public ActionResult<CourseViewModel> GetCourse(int courseId)
        {
            Course course = _dataRepository.GetCourse(courseId);
            if (course == null)
            {
                return NotFound();
            }

            var courseCategories = _dataRepository.GetCourseCategories(courseId);
            int userId = 3;  // dummy user_id 
            var lastLessonId = _dataRepository.GetLastLessonId(courseId, userId);

            return Ok(new CourseViewModel
            {   
               Course = course,
               CourseCategories = courseCategories.ToList(),
               LastLessonId = lastLessonId
            });
        }

        [HttpGet("{courseId}/lesson/{lessonId}")]
        public ActionResult<Lesson> GetLesson(int courseId, int lessonId)
        {
            int userId = 5;  // dummy user_id

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
