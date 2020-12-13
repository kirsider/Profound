using Profound.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Profound.Data.ViewModels
{
    public class CourseViewModel
    {
        public Course course { get; set; }
        public IEnumerable<Category> CourseCategories { get; set; }
    }
}
