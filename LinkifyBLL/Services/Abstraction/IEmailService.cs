using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.Services.Abstraction
{
    public interface IEmailService
    {
        public Task SendEmail(string Receiver, string EmailSubject, string EmailBody="");
    }
}
