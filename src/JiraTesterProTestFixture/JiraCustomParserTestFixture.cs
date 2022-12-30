using JiraTesterProService;
using JiraTesterProService.JiraParser;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace JiraTesterProTestFixture
{
    [TestFixture]
    public class JiraCustomParserTestFixture:JiraTestFixtureBase
    {
        private IJiraCustomParser jiraParser;

        [SetUp]
        public void SetUp()
        { 
            jiraParser = _serviceProvider.GetService<IJiraCustomParser>();
        }
        [Test]
        public async Task Is_Able_ToGetJiraFieldDetailsForProject()
        {
            
            var parsedResult = await jiraParser.GetParsedJiraRootBasedOnProject("ED");

            Assert.NotNull(parsedResult);
        }
        [Test]
        public async Task Is_Able_ToGetJiraFieldDetailsForIssueID()
        {

            var clientprovider = _serviceProvider.GetService<IJiraClientProvider>();
            var issue = await clientprovider.GetJiraClient().Issues.GetIssueAsync("ED-5755");
                //.GetIssuesFromJqlAsync(new IssueSearchOptions("project = ED AND issuetype = Bug"));
            //    var anyrandomIssue = issue;//.FirstOrDefault();
            //if (anyrandomIssue != null)
            //{
            //    var transitions = await anyrandomIssue.GetAvailableActionsAsync(true);
            //    //foreach (var VARIABLE in transitions)
            //    //{
            //    //    var x = VARIABLE;
                    
            //    //}
            //    var parsedResult = await jiraParser.GetParsedJiraFieldsBasedOnIssueKey(anyrandomIssue.Key.Value);
            //    var editmeta = await anyrandomIssue.GetIssueFieldsEditMetadataAsync();

            //    Assert.NotNull(parsedResult);
            //}
            
        }

        [Test]
        public async Task Is_Able_To_GetClient()
        {
            var client = await jiraParser.GetJiraMetaData();
            Assert.IsNotNull(client);
        }

    }
}
