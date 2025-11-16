using FluentResults;
using User.Domain.Entities;

namespace User.Domain.Ports
{
    public interface IUserValidator
    {
        Result Validate(UserEntity user);
    }
}
