using System.Data;

namespace AuthService.Factories.Interface
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection(string connectionString);   
    }
}
