namespace SharedKernel.Events.Publication
{
    // Evento emitido por PublicationService al eliminar una publicación
    public class PublicationDeletedEvent
    {
        public int PublicationId { get; set; }
    }
}
