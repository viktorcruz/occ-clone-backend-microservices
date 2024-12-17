using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Events.Publication
{
    // Evento emitido por PublicationService al actualizar una publicación
    public class PublicationUpdatedEvent
    {
        public int IdPublication { get; set; }
        public int IdUser { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
