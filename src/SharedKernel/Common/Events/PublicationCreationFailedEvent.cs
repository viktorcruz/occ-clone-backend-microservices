using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Common.Events
{
    public class PublicationCreationFailedEvent
    {
        public string Reason { get; set; }
        public string PerformedBy { get; set; }
    }
}
