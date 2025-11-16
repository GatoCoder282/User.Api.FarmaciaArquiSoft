using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace User.Infraestructure.Persistence
{
    public sealed class SmtpOptions
    {
        public string Host { get; set; } = "";
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;     // TLS
        public string User { get; set; } = "";
        public string Password { get; set; } = "";
        public string From { get; set; } = "";
        public string FromName { get; set; } = "No-Reply";
    }

    public sealed class EmailSender : IEmailSender
    {
        private readonly SmtpOptions _opt;

        public EmailSender(IOptions<SmtpOptions> options)
            => _opt = options.Value;

        public async Task SendAsync(string to, string subject, string body, bool isHtml = false, CancellationToken ct = default)
        {
            using var client = new SmtpClient(_opt.Host, _opt.Port)
            {
                EnableSsl = _opt.EnableSsl,
                Credentials = new NetworkCredential(_opt.User, _opt.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Timeout = 10000
            };

            var msg = new MailMessage
            {
                From = new MailAddress(_opt.From, _opt.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            msg.To.Add(to);

            using (msg)
            {
                ct.ThrowIfCancellationRequested();
#if NET6_0_OR_GREATER
                await client.SendMailAsync(msg, ct);
#else
                await client.SendMailAsync(msg);
#endif
            }
        }
    }
}
