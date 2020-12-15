using Profound.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Profound.Data.ViewModels
{
    public class PostLessonViewModel
    {
        public IEnumerable<UserSolution> Solutions { get; set; }

        public int CourseId { get; set; }
        public int LessonId { get; set; }
    }
}
