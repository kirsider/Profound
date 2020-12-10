using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Profound.Data.Models
{
    public class Module
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }

        public int Order { get; set; }

        public IEnumerable<Lesson> Lessons { get; set; }
    }
}
