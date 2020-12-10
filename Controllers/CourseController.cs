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
            var course = _dataRepository.GetCourse(courseId);
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
        public IEnumerable<Lesson> GetLessons(int moduleId)
        {
            return _dataRepository.GetModuleLessons(moduleId);
        }

        [HttpGet("{courseId}/modules/{moduleId}/{lessonId}")]
        public IEnumerable<Component> GetComponents(int lessonId)
        {
            return _dataRepository.GetLessonComponents(lessonId);
        }

        public IEnumerable<Comment> GetComments(int component_id)
        {
            return _dataRepository.GetCommentsFromComponent(component_id);
        }


        [HttpPost("enrollment")]
        public ActionResult<Course> PostEnrollment(UserCourseEnrollment userCourseEnrollment)
        {
            var course = _dataRepository.PostEnrollment(userCourseEnrollment);
            return CreatedAtAction("PostEnrollment", course);
        }
    }
}
