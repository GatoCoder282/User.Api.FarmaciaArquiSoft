using System;
using System.Linq;
using User.Domain.Entities;
using User.Domain.Enums;
using User.Domain.Ports;

namespace User.Application.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        public bool CanPerformAction(UserEntity user, string action)
        {
            if (user.role == UserRole.Administrador) return true;
            var allowed = user.role switch
            {
                UserRole.Cajero => new[] { "Vender", "VerMisDatos" },
                UserRole.Almacenero => new[] { "Almacenar", "VerMisDatos" },
                _ => Array.Empty<string>()
            };
            return allowed.Contains(action, StringComparer.OrdinalIgnoreCase);
        }
    }
}
