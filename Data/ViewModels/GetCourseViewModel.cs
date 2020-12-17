using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Profound.Data.Models;

namespace Profound.Data.ViewModels
{
    public class GetCourseViewModel
    {
        public int Id { get; set; }
        public CourseUserViewModel Creator { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double AcceptancePercantage { get; set; }
        public double Progress { get; set; }
        public string Status { get; set; }
        public string RejectMessage { get; set; }

        public string Requirements { get; set; }

        public DateTime PublishedAt { get; set; }

        public IEnumerable<Module> Modules { get; set; }
    }
}
