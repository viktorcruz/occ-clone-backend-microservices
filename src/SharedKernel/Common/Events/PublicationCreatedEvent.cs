﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Common.Events
{
    // Evento emitido por PublicationService al crear una publicación
    public class PublicationCreatedEvent
    {
        public int IdPublication { get; set; }
        public int IdUser { get; set; } // El ID del usuario que creó la publicación
        public string Title { get; set; }
        public string Description { get; set; }
        //
        public DateTime ExpirationDate { get; set; }
        public int Status { get; set; }
        public decimal Salary { get; set; }
        public string Company { get; set; }
    }
}