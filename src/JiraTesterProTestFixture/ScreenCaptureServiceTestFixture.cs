using JiraTesterProService.ImageHandler;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraTesterProTestFixture
{
    [TestFixture]
    public class ScreenCaptureServiceTestFixture:JiraTestFixtureBase
    {

        [Test]
        public async Task Is_Able_To_TakeScreenCapture()
        {
            var screencaptureService = _serviceProvider.GetService<IScreenCaptureService>();
            var result = await screencaptureService.CaptureScreenShot("https://jiradev.ert.com/jira/browse/TRAIN-1", "c:\\temp\\test.png");
          Assert.IsTrue(result);
        }
    }
}
