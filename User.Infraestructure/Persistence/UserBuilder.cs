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
   
        public class UserBuilder : IUserBuilder
        {
            private readonly UserEntity _u = new();

            public IUserBuilder WithNames(string firstName, string lastFirstName, string? lastSecondName)
            {
                _u.first_name = firstName?.Trim() ?? "";
                _u.last_first_name = lastFirstName?.Trim() ?? "";
                _u.last_second_name = string.IsNullOrWhiteSpace(lastSecondName) ? null : lastSecondName!.Trim();
                return this;
            }

            public IUserBuilder WithContact(string mail, string phone, string ci)
            {
                _u.mail = mail?.Trim() ?? "";
                _u.phone = phone?.Trim() ?? "";
                _u.ci = ci?.Trim() ?? "";
                return this;
            }

            public IUserBuilder WithRole(UserRole role)
            {
                _u.role = role;
                return this;
            }

            public IUserBuilder WithAudit(int actorId)
            {
                _u.created_by = actorId;
                _u.updated_by = actorId;
                _u.created_at = DateTime.Now;
                _u.updated_at = DateTime.Now;
                _u.is_deleted = false;
                return this;
            }

            public IUserBuilder WithUsername(string username)
            {
                _u.username = username ?? "";
                return this;
            }

            public IUserBuilder WithPasswordHash(string passwordHash)
            {
                _u.password = passwordHash ?? "";
                return this;
            }

            public UserEntity Build() => _u;
        }
}

