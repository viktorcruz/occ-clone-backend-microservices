using Microsoft.Data.SqlClient;
using RolesServices.Persistence.Interface;
using System.Data;

namespace RolesServices.Persistence.Data
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
                throw new ArgumentException("La cadena de conexion no puede ser nul a vacia", nameof(connectionName));
            }
            return new SqlConnection(connectionString);
        }
        #endregion
    }
}
