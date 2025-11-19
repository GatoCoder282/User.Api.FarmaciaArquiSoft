using User.Domain.Entities;

namespace User.Domain.Ports
{
    public interface ITokenService
    {
        string GenerateToken(UserEntity user);
    }
}