using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Common.Events
{
    // Evento emitido por PublicationService al actualizar una publicación
    public class PublicationUpdatedEvent
    {
        public int PublicationId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
