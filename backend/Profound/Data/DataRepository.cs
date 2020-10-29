using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Profound.Data.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace Profound.Data
{
    public class DataRepository: IDataRepository
    {
        private readonly string _connectionString;

        public DataRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:MySQLConnection"];
        }

        public IEnumerable<Role> GetRoles()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<Role>(
                    @"SELECT id, role as roleName FROM Role;"
                );
            }
        }

    }
}
