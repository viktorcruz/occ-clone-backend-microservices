using System.Data;

namespace SharedKernel.Common.Interfaces.Persistence
{
    public interface ISqlServerConnectionFactory
    {
        IDbConnection GetConnection(string connectionName);
    }
}
