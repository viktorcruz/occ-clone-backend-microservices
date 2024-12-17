using Microsoft.Extensions.Configuration;
using SharedKernel.Common.Interfaces.Persistence;
using System.Data;
using System.Data.SqlClient;

namespace SharedKernel.Data
{
    public class SqlServerConnectionFactory : ISqlServerConnectionFactory
    {
        #region Properties
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        public SqlServerConnectionFactory(IConfiguration configuration)
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
