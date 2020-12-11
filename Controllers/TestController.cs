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
    public class TestController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;

        public TestController(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            return _dataRepository.GetUsers();
        }

        [HttpGet("{userId}")]
        public ActionResult<User> GetUser(int userId)
        {
            var user = _dataRepository.GetUser(userId);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpGet("roles")]
        public IEnumerable<Role> GetRoles()
        {
            return _dataRepository.GetRoles();
        }

        [HttpGet("courses")]
        public IEnumerable<Course> GetCourses()
        {
            return _dataRepository.GetCourses();
        }

        [HttpGet("courses/{courseId}")]
        public ActionResult<Course> GetCourse(int courseId)
        {
            var course = _dataRepository.GetCourse(courseId);
            if (course == null)
            {
                return NotFound();
            }
            return course;
        }

        [HttpGet("categories/{categoryId}")]
        public ActionResult<Category> GetCategory(int categoryId)
        {
            var category = _dataRepository.GetCategory(categoryId);
            if (category == null)
            {
                return NotFound();
            }
            return category;
        }

    }
}
