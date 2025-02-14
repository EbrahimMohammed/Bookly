using Bookly.Application.Abstractions.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Infrastructure.Email
{
    public sealed class EmailService : IEmailService
    {
        public Task SendAsync(Domain.Users.Email email, string subject, string body)
        {
            throw new Exception("Error while sending email");
        }
    }
}
