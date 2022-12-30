using JiraTesterProService.ImageHandler;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraTesterProData;
using JiraTesterProData.JiraMapper;

namespace JiraTesterProTestFixture
{
    [TestFixture]
    public class ScreenCaptureServiceTestFixture:JiraTestFixtureBase
    {
        [SetUp]
        public void SetUp()
        {
            screencaptureService = _serviceProvider.GetService<IScreenCaptureService>();
        }
        private IScreenCaptureService screencaptureService;
        [Test]
        public async Task Is_Able_To_TakeScreenCapture()
        {
            await screencaptureService.SetStartSession();
            var result = await screencaptureService.CaptureScreenShot(new ScreenShotInputDto()
            {
                TestUrl = "https://google.com", FilePath = "C:\\temp\\testscreenshot.png"
            });
            Assert.IsTrue(result);

            await screencaptureService.CloseSession();
        }

        //[Test]
        //public async Task Is_Able_To_Create_Issue_For_TelecomRequest()
        //{
        //    var comparer = StringComparer.OrdinalIgnoreCase;

        //    var suppliedscreenDto = new Dictionary<string, string>(comparer)
        //    {
        //        { "Project", "oneECOA DEV (ED)" },
        //        { "Issue Type", "Telecom Request" },
        //        {"Component/s","ERT Internal"},
        //        {"Telecom Request Type","New Request"},
        //        {"BYOD Request","No"},
        //        {"Training Video Link","http://www.google.com"},
        //       // {"Cognitive Debrief Date","2022-11-28"}
        //    };
        //    var result = await screencaptureService.CreateJira(new JiraTestMasterDto()
        //    {
        //        Project = "oneECOA DEV (ED)", IssueType = "Telecom Request", SuppliedValues = suppliedscreenDto
        //    }, JiraActionEnum.Update, new JiraFields(), new List<string>());

        //    Assert.IsTrue(result);
        //}

        //[Test]
        //public async Task Is_Able_To_Create_Issue_For_TelecomRequest_Update()
        //{
        //    var comparer = StringComparer.OrdinalIgnoreCase;

        //    var suppliedscreenDto = new Dictionary<string, string>(comparer)
        //    {
        //        { "Project", "oneECOA DEV (ED)" },
        //        { "Issue Type", "Telecom Request" },
        //        {"Component/s","ERT Internal"},
        //        {"Telecom Request Type","Existing/Update Request"},
        //        {"BYOD Request","Yes"},
        //        {"Training Video Link","http://www.google.com"},
        //         {"BYOD Buildtypes","iOS"}
        //    };
        //    var result = await screencaptureService.CreateJira(new JiraTestMasterDto()
        //    {
        //        Project = "oneECOA DEV (ED)",
        //        IssueType = "Telecom Request",
        //        SuppliedValues = suppliedscreenDto
        //    }, JiraActionEnum.Update, new JiraFields(), new List<string>());

        //    Assert.IsTrue(result);
        //}
        //[Test]
        //public async Task Is_Able_To_Create_Issue_For_Bug()
        //{
        //    var comparer = StringComparer.OrdinalIgnoreCase;

        //    var suppliedscreenDto = new Dictionary<string, string>(comparer)
        //    {
        //        { "Project", "oneECOA DEV (ED)" },
        //        { "Issue Type", "Bug" },
        //        {"Component/s","ERT Internal"},
               
        //        // {"Cognitive Debrief Date","2022-11-28"}
        //    };
        //    var result = await screencaptureService.CreateJira(new JiraTestMasterDto()
        //    {
        //        Project = "oneECOA DEV (ED)",
        //        IssueType = "Bug",
        //        SuppliedValues = suppliedscreenDto
        //    }, JiraActionEnum.Update, new JiraFields(), new List<string>());

        //    Assert.IsTrue(result);
        //}
        //[Test]
        //public async Task Is_Able_To_Edit_Issue_For_Bug()
        //{
        //    var comparer = StringComparer.OrdinalIgnoreCase;

        //    var suppliedscreenDto = new Dictionary<string, string>(comparer)
        //    {
        //        { "Project", "oneECOA DEV (ED)" },
        //        { "Issue Type", "Bug" },
        //        {"Component/s","ERT Internal"},
               
        //        // {"Cognitive Debrief Date","2022-11-28"}
        //    };
        //    var result = await screencaptureService.EditJira(new JiraTestMasterDto()
        //    {
        //        Project = "oneECOA DEV (ED)",
        //        IssueType = "Bug",
        //        SuppliedValues = suppliedscreenDto, Status = "Reopen Issue",IssueKey = "ED-6105"

        //    }, JiraActionEnum.Update, new JiraFields(), new List<string>());

        //    Assert.IsTrue(result);
        //}
    }
}
