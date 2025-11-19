using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Domain.Entities;

namespace User.Domain.Ports
{
    public interface IUserFacade
    {
        Task<UserEntity> RegisterAsync(UserEntity user, int actorId);
        Task<UserEntity?> GetByIdAsync(int id);
        Task<IEnumerable<UserEntity>> ListAsync();
        Task UpdateAsync(UserEntity user, int actorId);
        Task SoftDeleteAsync(int id, int actorId);
        Task<UserEntity> AuthenticateAsync(string username, string password);
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        bool CanPerformAction(UserEntity user, string action);
    }
}
