using MediatR;
using SharedKernel.Common.Interfaces;
using SharedKernel.Interface;

namespace UsersService.Application.Commands.Handlers
{
    public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly IEventBus _eventBus;
        #endregion

        #region Constructor
        public ChangeUserPasswordCommandHandler(
            IApplicationExceptionHandler applicationExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse,
            IEventBus eventBus
            )
        {
            _applicationExceptionHandler = applicationExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventBus = eventBus;

        }
        #endregion

        #region Methods
        public Task<IEndpointResponse<IDatabaseResult>> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            //try
            //{

            throw new NotImplementedException();
            //}
            //catch ( Exception ex )
            //{
            //_applicationExceptionHandler.HandleGenericException<string>(ex, "ChangeUserPasswordHandler");
            //_endpointResponse.Message = $"Error changing password: {ex.Message}";
            //_endpointResponse.IsSuccess = false;

            //}
            //return _endpointResponse;
        }

        #endregion
    }
}
