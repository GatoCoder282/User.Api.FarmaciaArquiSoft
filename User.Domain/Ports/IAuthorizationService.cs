using User.Domain.Entities;

namespace User.Domain.Ports
{
    public interface IAuthorizationService
    {
        bool CanPerformAction(UserEntity user, string action);
    }
}