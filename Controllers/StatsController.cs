using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Profound.Data;
using Profound.Data.Models;

namespace Profound.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController: ControllerBase
    {
        private const int STATS_PAGE_LIMIT = 10;

        private readonly IDataRepository _dataRepository;

        public StatsController(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        [HttpGet("course/{courseId}")]
        public CourseStats GetCourseStats(int courseId, int offset=0, int limit=STATS_PAGE_LIMIT)
        {
            return _dataRepository.GetCourseStats(courseId, offset, limit);
        }

        [HttpGet("component/{componentId}")]
        public ComponentStats GetComponentStats(int componentId)
        {
            return _dataRepository.GetComponentStats(componentId);
        }
    }
}
