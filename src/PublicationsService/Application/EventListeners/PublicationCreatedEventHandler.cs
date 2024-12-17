using PublicationsService.Aplication.Commands;
using PublicationsService.Aplication.DTO;
using PublicationsService.Infrastructure.Interface;
using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Events.Publication;

namespace PublicationsService.Application.EventListeners
{
    public class PublicationCreatedEventHandler : IEventHandler<PublicationCreatedEvent>
    {
        private readonly IPublicationRepository _publicationRepository;

        public PublicationCreatedEventHandler(IPublicationRepository publicationRepository)
        {
            _publicationRepository = publicationRepository;
        }

        public async Task Handle(PublicationCreatedEvent @event)
        {
            var publication = new PublicationDTO
            {
                IdUser = @event.IdUser,
                Title = @event.Title,
                Description = @event.Description,
                ExpirationDate = @event.ExpirationDate,
                Status = @event.Status,
                Salary = @event.Salary,
                Location = @event.Location,
                Company = @event.Company,
            };
            CreatePublicationCommand publicationCommand = new CreatePublicationCommand(publication);

            await _publicationRepository.CreatePublicationAsync(publicationCommand);

            Console.WriteLine($"Publication created for user {@event.IdUser}");
        }
    }
}
