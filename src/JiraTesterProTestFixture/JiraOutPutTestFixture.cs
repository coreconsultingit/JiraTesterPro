using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlassian.Jira;
using JiraTesterProData;
using JiraTesterProService.OutputTemplate;

namespace JiraTesterProTestFixture
{
    [TestFixture]
    public class JiraOutPutTestFixture:JiraTestFixtureBase
    {

        [Test]
        public void GetFinalHtmlJiraOutPut()
        {
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
            var result = JiraTestOutput.GetJiraOutPutTemplate(jratestResult,new { title = "My Post", body = "test" });
            Assert.IsNotNull(result);
        }
    }
}
