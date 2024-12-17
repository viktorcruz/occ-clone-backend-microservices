using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Events.User
{
    // Evento emitido por UserService al eliminar un usuario
    public class UserDeletedEvent
    {
        public int UserId { get; set; }
    }
}
