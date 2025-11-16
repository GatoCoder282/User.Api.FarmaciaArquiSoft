using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Domain.Entities;
using User.Application.DTOs;   

namespace User.Application.Ports
{
    public interface IUserService
    {
        Task<UserEntity> RegisterAsync(UserCreateDto dto, int actorId);
        Task<UserEntity?> GetByIdAsync(int id);
        Task<IEnumerable<UserEntity>> ListAsync();
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task UpdateAsync(int id, UserUpdateDto dto, int actorId);
        Task SoftDeleteAsync(int id, int actorId);

        Task<UserEntity> AuthenticateAsync(string username, string password);

        bool CanPerformAction(UserEntity user, string action);
    }
}