using System.Data;

namespace RolesServices.Persistence.Interface
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection(string connectionName);
    }
}
