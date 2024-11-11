using Dapper;
using Moq;
using SharedKernel.Common.Interfaces;
using SharedKernel.Interface;
using System.Data;
using UsersService.Infrastructure.Repository;

namespace UsersService.UnitTest.Repositories
{
    public class EventLogRepositoryTests
    {
        [Fact]
        public async Task SaveEventLog_Should_Call_Stored_Procedure_With_Correct_Parameters()
        {
            // Arrange
            var mockDbConnection = new Mock<IDbConnection>();
            var mockDbTransaction = new Mock<IDbTransaction>();
            var mockGlobalExceptionHandler = new Mock<IGlobalExceptionHandler>();
            var mockDapperExecutor = new Mock<IDapperExecutor>();

            mockDbConnection.Setup(conn => conn.State).Returns(ConnectionState.Closed);
            mockDbConnection.Setup(conn => conn.Open());
            mockDbConnection.Setup(conn => conn.BeginTransaction()).Returns(mockDbTransaction.Object);

            // Mock IDbConnectionFactory
            var mockDbConnectionFactory = new Mock<ISqlServerConnectionFactory>();
            mockDbConnectionFactory.Setup(factory => factory.GetConnection(It.IsAny<string>()))
                .Returns(mockDbConnection.Object);

            // Mock ExecuteAsync de Dapper
            mockDapperExecutor.Setup(exec => exec.ExecuteAsync(
                mockDbConnection.Object,
                It.IsAny<string>(),
                It.IsAny<object>(),
                mockDbTransaction.Object,
                CommandType.StoredProcedure)).ReturnsAsync(1);

            var eventLogRepository = new EventLogRepository(
                mockDbConnectionFactory.Object, 
                mockGlobalExceptionHandler.Object,
                mockDapperExecutor.Object);

            var eventName = "TestEvent";
            var eventData = "{ \"Key\": \"Value\" }";
            var exchange = "TestExchange";
            var routingKey = "TestRoutingKey";

            // Act
            //await eventLogRepository.SaveEventLog(eventName, eventData, exchange, routingKey);


            // Assert
            mockDapperExecutor.Verify(exec => exec.ExecuteAsync(
                mockDbConnection.Object,
                "Usp_EventLog_Add",
                It.IsAny<DynamicParameters>(),
                mockDbTransaction.Object,
                CommandType.StoredProcedure),
                Times.Once);

            mockGlobalExceptionHandler.VerifyNoOtherCalls();
        }
    }
}