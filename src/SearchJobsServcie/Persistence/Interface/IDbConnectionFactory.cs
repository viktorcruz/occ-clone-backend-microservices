using System.Data;

namespace SearchJobsServcie.Persistence.Interface
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection(string connectionName);
    }
}
