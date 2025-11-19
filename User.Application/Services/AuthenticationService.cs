using System;
using System.Linq;
using System.Threading.Tasks;
using User.Domain.Entities;
using User.Domain.Exceptions;
using User.Domain.Ports;

namespace User.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordService _password;

        public AuthenticationService(IUserRepository repo, IPasswordService password)
        {
            _repo = repo;
            _password = password;
        }

        public async Task<UserEntity> AuthenticateAsync(string username, string password)
        {
            var all = await _repo.GetAll();
            var user = all.FirstOrDefault(u => u.username.Equals(username, StringComparison.OrdinalIgnoreCase) && !u.is_deleted);

            if (user is null || !_password.VerifyPassword(password, user.password))
                throw new DomainException("Credenciales inválidas.");

            return user;
        }
    }
}
