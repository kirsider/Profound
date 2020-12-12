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
    public class TeacherController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;

        public TeacherController(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        [HttpGet("courses")]
        public IEnumerable<Course> GetCourses()
        {
            int teacherId = 1;  // dummy id
            return _dataRepository.GetCourses().Where(c => c.CreatorId == teacherId);
        }

        [HttpGet("courses/{courseId}")]
        public ActionResult<Course> GetCourse(int courseId)
        {
            Course course = _dataRepository.GetCourse(courseId);

            if (course == null)
            {
                return NotFound();
            }
            return course;
        }

        [HttpPost("courses/create")]
        public ActionResult<Course> CreateCourse(CourseViewModel courseViewModel)
        {
            var course = _dataRepository.CreateCourse(courseViewModel.course);
            foreach (var category in courseViewModel.CourseCategories)
            {
                if (_dataRepository.GetCategory(category.Id) != null)
                    _dataRepository.CreateRecordingForCategoryCourse(course.Id, category.Id);
            }
            return CreatedAtAction("CreateCourse", course);
        }

        [HttpDelete("courses/{courseId}")]
        public ActionResult DeleteCourse(int id)
        {
            var course = _dataRepository.GetCourse(id);

            if (course == null)
            {
                return NotFound();
            }
            _dataRepository.DeleteСourse(id);
            return NoContent();
        }

        [HttpPost("courses/{course_id}/requestToPublish")]
        public void RequestToPublish(int course_id)
        {
            if (_dataRepository.GetBaseCourse(course_id) != null)
            {
                _dataRepository.RequestToPublish(course_id);
            }
        }

        [HttpPost("courses/{course_id}/publish")]
        public void PublishCourse(int course_id)
        {
            if (_dataRepository.GetBaseCourse(course_id) != null)
            {
                _dataRepository.PublishCourse(course_id);
            }
        }
    }
}
