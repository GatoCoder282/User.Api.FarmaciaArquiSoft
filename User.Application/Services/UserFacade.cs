using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using User.Domain.Entities;
using User.Domain.Enums;
using User.Domain.Exceptions;
using User.Domain.Ports;
using User.Domain.Validators;

namespace User.Application.Services
{
    public class UserFacade : IUserFacade
    {
        private readonly IUserRepository _repo;
        private readonly IUserValidator _validator;
        private readonly IEmailSender _email;
        private readonly IUserFactory _factory;
        private readonly IPasswordService _password;
        private readonly IAuthenticationService _auth;
        private readonly IUserPasswordService _pwd;
        private readonly IAuthorizationService _authz;

        public UserFacade(IUserRepository repo, IUserValidator validator, IEmailSender email, IUserFactory factory, IPasswordService password, IAuthenticationService auth, IUserPasswordService pwd, IAuthorizationService authz)

        {
            _repo = repo;
            _validator = validator;
            _email = email;
            _factory = factory;
            _password = password;
            _auth = auth;
            _pwd = pwd;
            _authz = authz;
        }

        // Recibe entidad parcialmente construida (sin username/password) o construida por factory
        public async Task<UserEntity> RegisterAsync(UserEntity user, int actorId)
        {
            if (string.IsNullOrWhiteSpace(user.mail))
                throw new DomainException("El correo es obligatorio.");

            // Obtener usernames existentes
            var existing = await _repo.GetAll();

            // Si el caller no creó username, la factory puede hacerlo. Podemos delegar:
            var baseUsernames = existing.Select(x => x.username);

            // Usar factory: nota: la factory asume parámetros primitivos, así que delegamos sus valores
            var candidateEntity = _factory.CreateForRegister(user.first_name, user.last_first_name, user.last_second_name, user.mail, user.phone, user.ci, actorId, baseUsernames, (int)user.role);

            // generar password y hashearla
            var plainPassword = _password.GenerateRandomPassword(12);
            candidateEntity.password = _password.HashPassword(plainPassword);
            candidateEntity.has_changed_password = false;
            candidateEntity.password_version = 1;

            // Validar dominio
            var pre = _validator.Validate(candidateEntity);
            if (!pre.IsSuccess) throw new ValidationException(pre.Errors.ToDictionary());

            // comprobaciones de unicidad (por seguridad)
            if (existing.Any(x => x.ci.Equals(candidateEntity.ci, StringComparison.OrdinalIgnoreCase)))
                throw new DomainException("El CI ya existe.");
            if (existing.Any(x => !string.IsNullOrWhiteSpace(x.mail) && x.mail!.Equals(candidateEntity.mail, StringComparison.OrdinalIgnoreCase)))
                throw new DomainException("El correo ya existe.");

            // Persistir
            var created = await _repo.Create(candidateEntity);

            // Enviar correo (best effort)
            var subject = "Tu acceso al sistema de la farmacia";
            var body = $@"Hola {created.first_name},

Se creó tu cuenta.
Usuario: {created.username}
Contraseña temporal: {plainPassword}

Por seguridad, cambia la contraseña al ingresar.";

            try { await _email.SendAsync(created.mail!, subject, body); } catch { /* log y seguir */ }

            return created;
        }

        public Task<UserEntity?> GetByIdAsync(int id) => _repo.GetById(new UserEntity { id = id });

        public Task<IEnumerable<UserEntity>> ListAsync() => _repo.GetAll();

        public async Task UpdateAsync(UserEntity user, int actorId)
        {
            var current = await GetByIdAsync(user.id) ?? throw new NotFoundException("Usuario no encontrado.");

            current.first_name = user.first_name;
            current.last_first_name = user.last_first_name;
            current.last_second_name = user.last_second_name;
            current.mail = user.mail;
            current.phone = user.phone;
            current.ci = user.ci;
            current.role = user.role;

            current.updated_by = actorId;
            current.updated_at = DateTime.Now;

            var existingUsers = await _repo.GetAll();

            if (existingUsers.Any(u => u.ci.Equals(current.ci, StringComparison.OrdinalIgnoreCase) && u.id != current.id))
                throw new DomainException("El CI ya existe.");

            if (!string.IsNullOrWhiteSpace(current.mail) && existingUsers.Any(u => !string.IsNullOrWhiteSpace(u.mail) && u.mail!.Equals(current.mail, StringComparison.OrdinalIgnoreCase) && u.id != current.id))
                throw new DomainException("El correo ya existe.");

            var result = _validator.Validate(current);
            if (!result.IsSuccess) throw new ValidationException(result.Errors.ToDictionary());

            await _repo.Update(current);
        }

        public Task ChangePasswordAsync(int userId, string currentPassword, string newPassword) => _pwd.ChangePasswordAsync(userId, currentPassword, newPassword);


        public async Task SoftDeleteAsync(int id, int actorId)
        {
            var current = await GetByIdAsync(id) ?? throw new NotFoundException("Usuario no encontrado.");
            current.is_deleted = true;
            current.updated_by = actorId;
            current.updated_at = DateTime.Now;
            await _repo.Delete(current);
        }

        public async Task<UserEntity> AuthenticateAsync(string username, string password) => await _auth.AuthenticateAsync(username, password);

        public bool CanPerformAction(UserEntity user, string action) => _authz.CanPerformAction(user, action);
    }
} 

