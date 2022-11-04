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
        public void GetFinalHtmlJiraOutPut()
        {
            var jiraoutput = _serviceProvider.GetService<IJiraTestOutputGenerator>();
            var jratestResult = new List<JiraTestResult>()
            {
                new()
                {
                    ExceptionMessage = "",HasException = false,TestPassed = true,JiraTestMasterDto = new JiraTestMasterDto()
                    {
                        Action = JiraActionEnum.Create.ToString(),Expectation = JiraTestStatusEnum.Passed.ToString(), StepId = 1,
                        

                    }, Comment = "Test"
                },
                new()
                {
                    ExceptionMessage = "Exception",HasException = true,TestPassed = false,JiraTestMasterDto = new JiraTestMasterDto()
                    {
                        Action = JiraActionEnum.Create.ToString(),Expectation = JiraTestStatusEnum.Passed.ToString(), StepId = 2,


                    }, Comment = "Test1"
                }
            };
            var result = jiraoutput.GetJiraOutPutTemplate(jratestResult);
            Assert.IsNotNull(result);
        }
    }
}
