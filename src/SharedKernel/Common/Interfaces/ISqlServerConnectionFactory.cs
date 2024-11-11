using System.Data;

namespace SharedKernel.Common.Interfaces
{
    public interface ISqlServerConnectionFactory
    {
        IDbConnection GetConnection(string connectionName);
    }
}
