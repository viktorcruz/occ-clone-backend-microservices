using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Events.Publication
{
    public class PublicationFailedEvent
    {
        public int IdUser { get; set; }
        public int IdRole { get; set; }
        public string Title { get; set; }
        public string Reason { get; set; }
        public DateTime FailedAt { get; set; }
    }
}
