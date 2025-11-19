using System;
using System.Threading.Tasks;
using User.Domain.Entities;
using User.Domain.Exceptions;
using User.Domain.Ports;

namespace User.Application.Services
{
    public class UserPasswordService : IUserPasswordService
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordService _password;

        public UserPasswordService(IUserRepository repo, IPasswordService password)
        {
            _repo = repo;
            _password = password;
        }

        public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var u = await _repo.GetById(new UserEntity { id = userId }) ?? throw new NotFoundException("Usuario no encontrado.");

            if (!_password.VerifyPassword(currentPassword, u.password))
                throw new DomainException("La contraseña actual no es correcta.");

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
                throw new DomainException("La nueva contraseña debe tener al menos 8 caracteres.");

            if (_password.VerifyPassword(newPassword, u.password))
                throw new DomainException("La nueva contraseña no puede ser igual a la anterior.");

            u.password = _password.HashPassword(newPassword);
            u.has_changed_password = true;
            u.password_version += 1;
            u.last_password_changed_at = DateTime.Now;
            u.updated_at = DateTime.Now;

            await _repo.Update(u);
        }
    }
}
