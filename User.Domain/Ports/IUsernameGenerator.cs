using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Domain.Ports
{
    public interface IUsernameGenerator
    {
        string GenerateBase(string firstName, string lastFirstName, string? lastSecondName);
        string EnsureUnique(string baseUsername, IEnumerable<string> existing);
    }
}
