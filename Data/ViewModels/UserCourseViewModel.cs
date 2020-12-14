using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Profound.Data.Models;

namespace Profound.Data.ViewModels
{
    public class UserCourseViewModel
    {
        public Course Course { get; set; }

        public IEnumerable<Category> CourseCategories { get; set; }
        public int LastLessonId { get; set; }
    }
}
