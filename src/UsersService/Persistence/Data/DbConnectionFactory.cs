using Microsoft.AspNetCore.Connections;
using Microsoft.Data.SqlClient;
using System.Data;
using UsersService.Persistence.Interface;

namespace UsersService.Persistence.Data
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        #region Properties
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region Methods
        public IDbConnection GetConnection(string connectionName)
        {
            var connectionString = _configuration.GetConnectionString(connectionName);
            if (connectionString == null)
            {
                throw new ArgumentException("Connection name must not be empty", nameof(connectionName));
            }
            return new SqlConnection(connectionString);
        }
        #endregion
    }
}
