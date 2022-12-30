using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraTesterProService.MailHandler;

namespace JiraTesterProTestFixture
{
    [TestFixture]
    public class EmailSenderTestFixture:JiraTestFixtureBase
    {


        [Test]
        public void No_Exeption_On_Mail()
        {
            var mail = _serviceProvider.GetService<IEmailSender>();
            Assert.DoesNotThrow(()=>mail.SendMessage("sunil.kumar.cci@clario.com","test","test",null));
        }
    }
}
