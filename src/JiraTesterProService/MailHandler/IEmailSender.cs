using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraTesterProService.MailHandler
{
    public interface IEmailSender
    {
        bool? IsBodyHtml { get; set; }
        void SendMessage(string to, string subject, string body, params string[] attachments);
    }
}
