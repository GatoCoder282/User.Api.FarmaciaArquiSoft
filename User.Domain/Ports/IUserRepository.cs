using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Domain.Entities;

namespace User.Domain.Ports
{
    public interface IUserRepository
    {
        Task<UserEntity> Create(UserEntity user);
        Task<UserEntity?> GetById(UserEntity user);
        Task<IEnumerable<UserEntity>> GetAll();
        Task Update(UserEntity user);
        Task Delete(UserEntity user);
    }
}
