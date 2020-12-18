using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Profound.Data.Models
{
    public class UserSolution
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }        
        public string Status { get; set; }
        public int Points { get; set; }

        public string Answer { get; set; }
    }
}
