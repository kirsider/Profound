using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Profound.Data.Models;

namespace Profound.Data.ViewModels
{
    public class ComponentViewModel
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string ComponentType { get; set; }
        public int MaxPoints { get; set; }
        public string Content { get; set; }

        public int Order{ get; set; }
        public bool Completed { get; set; }

        public IEnumerable<Comment> Comments { get; set; }
    }
}
