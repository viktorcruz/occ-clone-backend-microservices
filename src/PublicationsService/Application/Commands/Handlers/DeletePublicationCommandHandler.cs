﻿using MediatR;
using PublicationsService.Domain.Interface;
using SharedKernel.Common.Extensions;
using SharedKernel.Common.Interfaces;
using SharedKernel.Interface;

namespace PublicationsService.Aplication.Commands.Handlers
{
    public class DeletePublicationCommandHandler : IRequestHandler<DeletePublicationCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IPublicationDomain _publicationDomain;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        #endregion

        #region Constructor
        public DeletePublicationCommandHandler(
            IPublicationDomain publicationDomain,
            IEventPublisherService eventPublisherService,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse
            )
        {
            _publicationDomain = publicationDomain;
            _eventPublisherService = eventPublisherService;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(DeletePublicationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _publicationDomain.DeletePublicationByIdAsync(request.IdPublication);
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Publication deleted successful";

                    var additionalData = new
                    {
                        IdPublication = request.IdPublication,
                        IsDeleted = true
                    };

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "Publication",
                        operationType: "DELETE",
                        success: true,
                        performedBy: "Admin",
                        reason: response.ResultMessage,
                        additionalData: additionalData,
                        exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Delete_Success.ToRoutingKey()
                        );
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = response?.ResultMessage ?? "Publication not found";

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "Publication",
                        operationType: "DELETE",
                        success: false,
                        performedBy: "Admin",
                        reason: response?.ResultMessage ?? "Publication not found",
                        additionalData: null,
                        exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Delete_Failed.ToRoutingKey()
                        );
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "DeletePublicationCommand.Handle");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error deleting publication: {ex.Message}";

                await _eventPublisherService.PublishEventAsync(
                        entityName: "Publication",
                        operationType: "DELETE",
                        success: false,
                        performedBy: "Admin",
                        reason: ex.Message,
                        additionalData: null,
                        exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Delete_Error.ToRoutingKey()
                        );
            }
            return _endpointResponse;
        }
        #endregion
    }
}
