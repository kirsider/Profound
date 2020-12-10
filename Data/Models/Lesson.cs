using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Profound.Data.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public string Name { get; set; }

        public int Order { get; set; }

        public IEnumerable<Component> Components { get; set; }
    }
}
