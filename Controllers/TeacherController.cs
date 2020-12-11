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
            Course course =  _dataRepository.GetCourse(courseId);

            if (course == null)
            {
                return NotFound();
            }
            return course;
        }

        [HttpPost("courses/create")]
        public Course CreateCourse(Course course)
        {
            return _dataRepository.CreateCourse(course);
        }

        [HttpDelete("courses/{courseId}")]
        public void DeleteCourse(int id)
        {
            _dataRepository.DeleteСourse(id);
        }

        [HttpPost("courses/{course_id}/requestToPublish")]
        public void RequestToPublish(int course_id)
        {
            _dataRepository.RequestToPublish(course_id);
        }

        [HttpPost("courses/{course_id}/publish")]
        public void PublishCourse(int course_id)
        {
            _dataRepository.PublishCourse(course_id);
        }
    }
}
