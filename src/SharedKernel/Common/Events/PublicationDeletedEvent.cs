using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Common.Events
{
    // Evento emitido por PublicationService al eliminar una publicación
    public class PublicationDeletedEvent
    {
        public int PublicationId { get; set; }
    }
}
