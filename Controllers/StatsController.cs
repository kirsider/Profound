using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Profound.Data;
using Profound.Data.Models;
using Profound.Data.ViewModels;

namespace Profound.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController: ControllerBase
    {
        private const int STATS_PAGE_LIMIT = 10;

        private readonly IDataRepository _dataRepository;
        private enum Ranks
        {
            Newbie = 1,
            Amateur = 5,
            Expert = 10
        }

        public StatsController(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        [HttpGet("course/{courseId}")]
        public ActionResult<CourseStats> GetCourseStats(int courseId, int offset=0, int limit=STATS_PAGE_LIMIT)
        {
            var userStats = _dataRepository.GetCourseStats(courseId, offset, limit);
            if (userStats == null)
            {
                return NotFound();
            }

            return userStats;
        }

        [HttpGet("component/{componentId}")]
        public ActionResult<ComponentStats> GetComponentStats(int componentId)
        {
            var componentStats = _dataRepository.GetComponentStats(componentId);
            if (componentStats == null)
            {
                return NotFound();
            }

            return componentStats;
        }


        [HttpGet("course/{courseId}/achievement")]
        public ActionResult<AchievementViewModel> GetUserRank(int courseId)
        {
            Course course = _dataRepository.GetBaseCourse(courseId);
            if (course == null)
            {
                return NotFound();
            }

            int userId = _dataRepository.GetUserIdByEmail(User.Identity.Name);
            var completedAmount = _dataRepository.GetCoursesCompletedByUser(courseId, userId);

            if (completedAmount < (int)Ranks.Amateur)
                return new AchievementViewModel{ Rank = Ranks.Newbie.ToString()};
            if (completedAmount < (int)Ranks.Expert)
                return new AchievementViewModel { Rank = Ranks.Amateur.ToString() };
            
            return new AchievementViewModel { Rank = Ranks.Expert.ToString() };
        }
    }
}
