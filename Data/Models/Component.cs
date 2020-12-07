using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Profound.Data.Models
{
    public class Component
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public int MaxPoints { get; set; }
        public string Content { get; set; }

        public IEnumerable<Comment> Comments { get; set; }
    }
}
