namespace UsersService.Saga.Interfaces
{
    public interface ICompensationActions
    {
        Task CompensateCreateUserAsync(IUserSagaContext sagaState);
    }
}
