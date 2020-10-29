using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Profound.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<char> GetString(string queryString)
        {
            queryString ??= "Hello, World!";
            foreach (char ch in queryString)
            {
                yield return ch;
            }
        }
    }
}
