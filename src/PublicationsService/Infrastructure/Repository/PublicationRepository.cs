using Dapper;
using PublicationsService.Aplication.Commands;
using PublicationsService.Aplication.Dto;
using PublicationsService.Application.Dto;
using PublicationsService.Infrastructure.Interface;
using SharedKernel.Common.Interfaces;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace PublicationsService.Infrastructure.Repository
{
    public class PublicationRepository : IPublicationRepository
    {
        #region Properties
        private readonly string OCC_Connection = "OCC_Connection";
        private readonly ISqlServerConnectionFactory _sqlServerConnection;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        #endregion

        #region Constructor
        public PublicationRepository(ISqlServerConnectionFactory sqlServerConnection, IGlobalExceptionHandler globalExceptionHandler)
        {
            _sqlServerConnection = sqlServerConnection;
            _globalExceptionHandler = globalExceptionHandler;
        }
        #endregion


        #region Methods
        public async Task<DatabaseResult> CreatePublicationAsync(CreatePublicationCommand command)
        {
            using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "Usp_Publications_Add";
                        var parameters = new DynamicParameters();
                        parameters.Add("@IdUser", command.IdUser);
                        parameters.Add("@IdRole", command.IdRole);
                        parameters.Add("@Title", command.Title);
                        parameters.Add("@Description", command.Description);
                        //parameters.Add("@PostedDate", command.PostedDate);
                        parameters.Add("@ExpirationDate", command.ExpirationDate);
                        parameters.Add("@Status", command.Status);
                        parameters.Add("@Salary", command.Salary);
                        parameters.Add("@Location", command.Location);
                        parameters.Add("@Company", command.Company);
                        parameters.Add("@IdJobType", command.IdJobType);

                        var results = await connection.QuerySingleAsync<DatabaseResult>(
                            query,
                            parameters,
                            transaction: transaction,
                            commandType: System.Data.CommandType.StoredProcedure
                            );

                        transaction.Commit();
                        if (results != null)
                        {
                            return results;
                        }

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _globalExceptionHandler.HandleGenericException<string>(ex, "PublicationRepository");
                        return new DatabaseResult
                        {
                            ResultStatus = false,
                            ResultMessage = ex.Message,
                            OperationType = "CREATE",
                            AffectedRecordId = 0,
                            OperationDateTime = DateTime.Now,
                            ExceptionMessage = ex.Message,
                        };
                    }
                    return new DatabaseResult
                    {
                        ResultStatus = false,
                        ResultMessage = "",
                        OperationType = "CREATE",
                        AffectedRecordId = 0,
                        OperationDateTime = DateTime.Now,
                        ExceptionMessage = "",
                    };
                }
            }
        }

        public async Task<RetrieveDatabaseResult<PublicationRetrieveDTO>> GetPublicationByIdAsync(int publicationId)
        {
            using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
            {
                try
                {
                    connection.Open();

                    var query = "Usp_Publications_Get";
                    var parameters = new DynamicParameters();
                    parameters.Add("@IdPublication", publicationId);
                    var results = await connection.QueryAsync<dynamic>(query, parameters, commandType: System.Data.CommandType.StoredProcedure);
                    var result = results.FirstOrDefault();

                    if (result != null)
                    {
                        var spResult = new RetrieveDatabaseResult<PublicationRetrieveDTO>
                        {
                            ResultStatus = result.ResultStatus,
                            ResultMessage = result.ResultMessage,
                            OperationType = result.OperationType,
                            AffectedRecordId = result.AffectedRecordId,
                            OperationDateTime = result.OperationDateTime,
                            ExceptionMessage = result.ExceptionMessage,
                            Details = new PublicationRetrieveDTO
                            {
                                IdPublication = result.IdPublication,
                                IdUser = result.IdUser,
                                IdRole = result.IdRole,
                                Title = result.Title,
                                Description = result.Description,
                                PublicationDate = result.PublicationDate,
                                ExpirationDate = result.ExpirationDate,
                                Status = result.Status,
                                Salary = result.Salary,
                                Location = result.Location,
                                Company = result.Company,
                            }
                        };

                        return spResult;
                    }
                }
                catch (Exception ex)
                {
                    _globalExceptionHandler.HandleGenericException<string>(ex, "PublicationRepository.GetPublicationById");
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
                return new RetrieveDatabaseResult<PublicationRetrieveDTO>();
            }
        }
        public async Task<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>> GetAllPublicationAsync()
        {
            try
            {
                using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
                {
                    connection.Open();

                    var query = "Usp_Publications_GetAll";
                    var results = await connection.QueryAsync<PublicationRetrieveDTO>(query, commandType: System.Data.CommandType.StoredProcedure);
                    var publications = results.ToList();
                    if (publications.Any())
                    {
                        return new RetrieveDatabaseResult<List<PublicationRetrieveDTO>>
                        {
                            Details = publications,
                            ResultStatus = true,
                            ResultMessage = "Publications retrieved successfully",
                            OperationType = "GET ALL",
                            OperationDateTime = DateTime.Now,
                            AffectedRecordId = 0,
                            ExceptionMessage = "No exceptions found"
                        };
                    }
                    else
                    {
                        return new RetrieveDatabaseResult<List<PublicationRetrieveDTO>>
                        {
                            Details = null,
                            ResultStatus = false,
                            ResultMessage = "No publicacionts found",
                            OperationType = "GET ALL",
                            OperationDateTime = DateTime.Now,
                            AffectedRecordId = 0,
                            ExceptionMessage = null
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "PublicationRepository");
                return new RetrieveDatabaseResult<List<PublicationRetrieveDTO>>
                {
                    Details = null,
                    ResultStatus = false,
                    ResultMessage = $"Error retrieving publications: {ex.Message}",
                    OperationType = "GET ALL",
                    OperationDateTime = DateTime.Now,
                    AffectedRecordId = 0,
                    ExceptionMessage = ex.Message
                };
            }
        }

        public async Task<DatabaseResult> UpdatePublicationAsync(PublicationUpdateDTO publicationDTO)
        {
            using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "Usp_Publications_Update";
                        var parameters = new DynamicParameters();
                        parameters.Add("@IdPublication", publicationDTO.IdPublication);
                        parameters.Add("@IdUser", publicationDTO.IdUser);
                        parameters.Add("@IdRole", publicationDTO.IdRole);
                        parameters.Add("@Description", publicationDTO.Description);
                        parameters.Add("@Status", publicationDTO.Status);
                        parameters.Add("@Salary", publicationDTO.Salary);
                        parameters.Add("@Location", publicationDTO.Location);
                        parameters.Add("@Company", publicationDTO.Company);

                        var results = await connection.QuerySingleAsync<DatabaseResult>(query, parameters, transaction, commandType: System.Data.CommandType.StoredProcedure);

                        transaction.Commit();

                        if (results != null)
                        {
                            return results;
                        }
                        else
                        {
                            return await Task.FromResult(new DatabaseResult
                            {
                                ResultStatus = false,
                                ResultMessage = "Publication not found",
                                OperationType = "UPDATE",
                                AffectedRecordId = 0,
                                OperationDateTime = DateTime.Now,
                                ExceptionMessage = "No exceptions found"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _globalExceptionHandler.HandleGenericException<string>(ex, "PublicationRepository.UpdatePublicationAsync");
                        return await Task.FromResult(new DatabaseResult
                        {
                            ResultStatus = false,
                            ResultMessage = "Publication not found",
                            OperationType = "UPDATE",
                            AffectedRecordId = 0,
                            OperationDateTime = DateTime.Now,
                            ExceptionMessage = ex.Message
                        });
                    }
                    finally
                    {
                        if (connection.State == System.Data.ConnectionState.Open)
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        public async Task<DatabaseResult> DeletePublicationByIdAsync(int publicationId)
        {
            using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "Usp_Publications_Delete";
                        var parameters = new DynamicParameters();
                        parameters.Add("@IdPublication", publicationId);

                        var results = await connection.QueryAsync<DatabaseResult>(query, parameters, transaction, commandType: System.Data.CommandType.StoredProcedure);
                        var result = results.FirstOrDefault();

                        transaction.Commit();

                        if (result.ResultStatus)
                        {
                            return result;
                        }
                        return null;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _globalExceptionHandler.HandleGenericException<string>(ex, "PublicationRepository.DeletePublicationAsync");
                        return new DatabaseResult { ResultStatus = false };
                    }
                    finally
                    {
                        if (connection.State == System.Data.ConnectionState.Open)
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }
        #endregion
    }
}
