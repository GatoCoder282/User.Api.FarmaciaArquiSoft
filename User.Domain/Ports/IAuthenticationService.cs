using System.Threading.Tasks;
using User.Domain.Entities;

namespace User.Domain.Ports
{
    public interface IAuthenticationService
    {
        Task<UserEntity> AuthenticateAsync(string username, string password);
    }
}