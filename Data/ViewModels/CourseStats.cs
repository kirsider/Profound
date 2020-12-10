using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Profound.Data.Models
{
    public class CourseStats 
    { 
        public IEnumerable<UserCoursePoints> UsersCourseStats { get; set; }
        public Pagination Pagination { get; set; }
    }
}
