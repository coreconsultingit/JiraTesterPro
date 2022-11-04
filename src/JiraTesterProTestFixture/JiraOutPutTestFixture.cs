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
                new JiraTestResult()
                {
                    ExceptionMessage = "",HasException = false,TestPassed = true,JiraTestMasterDto = new JiraTestMasterDto()
                    {
                        Action = JiraActionEnum.Create.ToString(),Expectation = JiraTestStatusEnum.Passed.ToString()
                    }
                }
            };
            var result = jiraoutput.GetJiraOutPutTemplate(jratestResult,new JiraMetaDataDto()
            {
                JiraUrl = "http:\\jiratest",
                JiraVersion = "1.0.0",
                TestFileName="MatrixFile",
                JiraAccount = "JiraAccountUser"

            });
            Assert.IsNotNull(result);
        }
    }
}
