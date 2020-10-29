using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Profound.Data.Models
{
    public class CommentPostRequest
    {
        public int ComponentId { get; set; }
        public string Text { get; set; }
    }
}
