using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Domain.Ports
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string body, bool isHtml = false, CancellationToken ct = default);
    }
}
