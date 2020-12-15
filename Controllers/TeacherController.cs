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

        [HttpGet("course/{courseId}")]
        public ActionResult<TeacherCourseViewModel> GetCourse(int courseId)
        {
            Course course = _dataRepository.GetCourse(courseId);
            if (course == null)
            {
                return NotFound();
            }

            var courseCategories = _dataRepository.GetCourseCategories(courseId);
            return Ok(new TeacherCourseViewModel
            {
                Course = course,
                CourseCategories = courseCategories.ToList()
            });
        }

        [HttpPost("courses")]
        public ActionResult<Course> CreateCourse(PostCourseViewModel courseViewModel)
        {
            courseViewModel.course.Status = "dev";
            var course = _dataRepository.CreateCourse(courseViewModel.course);
            foreach (var category in courseViewModel.CourseCategories)
            {
                if (_dataRepository.GetCategory(category.Id) != null)
                    _dataRepository.CreateCategoryCourse(course.Id, category.Id);
            }
            return CreatedAtAction("CreateCourse", course);
        }

        [HttpPost("course/modules")]
        public ActionResult<Course> CreateModule(Module module)
        {
            var course = _dataRepository.CreateModule(module);
            return CreatedAtAction("CreateModule", module);
        }

        [HttpDelete("course/{courseId}")]
        public ActionResult DeleteCourse(int course_id)
        {
            var course = _dataRepository.GetBaseCourse(course_id);
            if (course == null)
            {
                return NotFound();
            }

            _dataRepository.DeleteСourse(course_id);
            return NoContent();
        }

        [HttpPost("course/{courseId}/requestToPublish")]
        public ActionResult RequestToPublish(int courseId)
        {
            var course = _dataRepository.GetBaseCourse(courseId);
            if (course != null)
            {
                _dataRepository.RequestToPublish(courseId);
                return Ok(); 
            }
            return NotFound();
        }        
    }
}
