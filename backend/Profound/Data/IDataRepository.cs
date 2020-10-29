using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Profound.Data.Models;

namespace Profound.Data
{
    public interface IDataRepository
    {
        IEnumerable<Role> GetRoles();
    }
}
