using System.Threading.Tasks;

namespace User.Domain.Ports
{
    public interface IUserPasswordService
    {
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}