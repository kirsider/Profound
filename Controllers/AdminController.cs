using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Profound.Data;
using Profound.Data.ViewModels;

namespace Profound.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;

        public AdminController(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        [HttpPost("{courseId}/publish")]
        public ActionResult PublishCourse(int courseId)
        {
            var course = _dataRepository.GetBaseCourse(courseId);
            if (course != null)
            {
                _dataRepository.PublishCourse(courseId);
                return Ok();
            }
            return NotFound();
        }

        [HttpPost("{courseId}/reject")]
        public ActionResult RejectCourse(int courseId, [FromBody] RejectCourseViewModel vm)
        {
            var course = _dataRepository.GetBaseCourse(courseId);
            if (course != null)
            {
                _dataRepository.RejectCourse(vm.RejectMessage, courseId);
                return Ok();
            }
            return NotFound();
        }
    }
}