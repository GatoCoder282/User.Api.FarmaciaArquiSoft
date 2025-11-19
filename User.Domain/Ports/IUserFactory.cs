using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Domain.Entities;

namespace User.Domain.Ports
{
    
        public interface IUserFactory
        {
            // Crea entidad base (sin persistir). existingUsernames para asegurar unicidad.
            UserEntity CreateForRegister(string firstName, string lastFirstName, string? lastSecondName, string mail, string phone, string ci, int actorId, IEnumerable<string> existingUsernames, int role);
        }
    
}
