using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicy.UserManagement.Server.DataAccess.Configurations;

namespace Vicy.UserManagement.Server.DataAccess
{
    public class VicyDbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _writeConnectionString;
        private readonly string _readConnectionString;

        public VicyDbConnectionFactory(DbConnectionStrings connectionStrings)
        {
            if (string.IsNullOrEmpty(connectionStrings.Write))
                throw new ArgumentNullException(nameof(connectionStrings.Write));

            if (string.IsNullOrEmpty(connectionStrings.Read))
                throw new ArgumentNullException(nameof(connectionStrings.Read));

            _writeConnectionString = connectionStrings.Write;
            _readConnectionString = connectionStrings.Read;
        }

        public DbConnection CreateConnection(string nameOrConnectionString)
        {
            switch (nameOrConnectionString)
            {
                case "write":
                    return new SqlConnectionFactory().CreateConnection(_writeConnectionString);

                case "read":
                    return new SqlConnectionFactory().CreateConnection(_readConnectionString);

                default:
                    throw new ArgumentOutOfRangeException(nameof(nameOrConnectionString));
            }
        }
    }
}
