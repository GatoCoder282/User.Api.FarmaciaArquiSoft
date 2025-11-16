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
        Task Create(Entities.User user);
        Task<Entities.User?> GetById(Entities.User user);
        Task<IEnumerable<Entities.User>> GetAll();
        Task Update(Entities.User user);
        Task Delete(Entities.User user);
    }
}
