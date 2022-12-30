using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTesterProData;
using JiraTesterProService.OutputTemplate;
using Microsoft.Extensions.DependencyInjection;

namespace JiraTesterProTestFixture
{
    [TestFixture]
    public class JiraOutPutTestFixture:JiraTestFixtureBase
    {

        [Test]
        public async Task GetFinalHtmlJiraOutPut()
        {
            var jiraoutput = _serviceProvider.GetService<IJiraTestOutputGenerator>();
            var jratestResult = new List<JiraTestResult>()
            {
                new()
                {
                    ExceptionMessage = "",HasException = false,TestPassed = true,JiraTestMasterDto = new JiraTestMasterDto()
                    {
                        Action = JiraActionEnum.Create.ToString(),Expectation = JiraTestStatusConst.Passed, StepId = 1,GroupKey = "GP1"
                        

                    }, Comment = "Test",ScreenShotPath=directoryPath
                },
                new()
                {
                    ExceptionMessage = "",HasException = false,TestPassed = true,JiraTestMasterDto = new JiraTestMasterDto()
                    {
                        Action = JiraActionEnum.Create.ToString(),Expectation = JiraTestStatusConst.Failed, StepId = 2,GroupKey = "GP1"


                    }, Comment = "Test",ScreenShotPath=directoryPath
                },
                new()
                {
                    ExceptionMessage = "Exception",HasException = true,TestPassed = false,JiraTestMasterDto = new JiraTestMasterDto()
                    {
                        Action = JiraActionEnum.Create.ToString(),Expectation = JiraTestStatusConst.Passed, StepId = 2,GroupKey = "GP2"


                    }, Comment = "Test1",ScreenShotPath=directoryPath
                }
            };
            var result = await jiraoutput.GetJiraOutPutTemplate(jratestResult,DateTime.Now);
            Assert.IsNotNull(result);
        }
    }
}
