using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Profound.Data.Models
{
    public class UserCourseEnrollment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }

        public string Status { get; set; }

    }
}
