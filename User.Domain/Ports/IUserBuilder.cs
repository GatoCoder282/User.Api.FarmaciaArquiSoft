using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Domain.Entities;
using User.Domain.Enums;

namespace User.Domain.Ports
{
    
        public interface IUserBuilder
        {
            IUserBuilder WithNames(string firstName, string lastFirstName, string? lastSecondName);
            IUserBuilder WithContact(string mail, string phone, string ci);
            IUserBuilder WithRole(UserRole role);
            IUserBuilder WithAudit(int actorId);
            IUserBuilder WithUsername(string username);
            IUserBuilder WithPasswordHash(string passwordHash);
            UserEntity Build();
        }
    
}
