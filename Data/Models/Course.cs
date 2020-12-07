using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Profound.Data.Models
{
    public class Course
    {
        public int Id { get; set; }
        public int CreatorId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double AcceptancePercantage { get; set; }
        public string Status { get; set; }
        public DateTime PublishedAt { get; set; }

        public IEnumerable<Module> Modules { get; set; }
    }
}
