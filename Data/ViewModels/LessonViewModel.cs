using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Profound.Data.Models;

namespace Profound.Data.ViewModels
{
    public class LessonViewModel
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public string Name { get; set; }

        public int Order { get; set; }

        public bool Completed { get; set; }
        public IEnumerable<ComponentViewModel> Components { get; set; }        
    }
}
