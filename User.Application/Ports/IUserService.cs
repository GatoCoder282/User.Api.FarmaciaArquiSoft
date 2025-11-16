using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Domain.Entities; // Asegúrate de que 'User' esté definido aquí
using User.Application.DTOs;   

namespace User.Application.Ports
{
    public interface IUserService
    {
        Task<User> RegisterAsync(UserCreateDto dto, int actorId);
        Task<User?> GetByIdAsync(int id);
        Task<IEnumerable<User>> ListAsync();
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task UpdateAsync(int id, UserUpdateDto dto, int actorId);
        Task SoftDeleteAsync(int id, int actorId);

        Task<User> AuthenticateAsync(string username, string password);

        bool CanPerformAction(User user, string action);
    }
}