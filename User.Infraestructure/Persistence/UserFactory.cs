using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Domain.Entities;
using User.Domain.Enums;
using User.Domain.Ports;

namespace User.Infraestructure.Persistence
{
    public class UserFactory : IUserFactory
    {
        private readonly IUserBuilder _builder;
        private readonly IUsernameGenerator _usernameGenerator;

        public UserFactory(IUserBuilder builder, IUsernameGenerator usernameGenerator)
        {
            _builder = builder;
            _usernameGenerator = usernameGenerator;
        }

        public UserEntity CreateForRegister(string firstName, string lastFirstName, string? lastSecondName, string mail, string phone, string ci, int actorId, IEnumerable<string> existingUsernames, int role)
        {
            // Delegar generación y unicidad del username al servicio
            var baseUsername = _usernameGenerator.GenerateBase(firstName, lastFirstName, lastSecondName);
            var unique = _usernameGenerator.EnsureUnique(baseUsername, existingUsernames);

            var user = _builder
                .WithNames(firstName, lastFirstName, lastSecondName)
                .WithContact(mail, phone, ci)
                .WithRole((UserRole)role)
                .WithAudit(actorId)
                .WithUsername(unique)
                .Build();

            return user;
        }
    }
}
