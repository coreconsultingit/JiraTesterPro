using Atlassian.Jira;
using JiraTesterProService;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraTesterProTestFixture
{
    [TestFixture]
    public class JiraApiCallTestFixture: JiraTestFixtureBase
    {
        [Test,Ignore("Only for local testing")]
        public async Task GetFieldDetailsForProject()
        {
            var clientprovider = _serviceProvider.GetService<IJiraClientProvider>();
            var issue = await clientprovider.GetJiraClient().Fields.GetCustomFieldsAsync(); //.GetIssueAsync("ED-5420");
            var fieldDetails = issue.Where(x => x.Id.Contains("12957"));

            var issueTypes = await clientprovider.GetJiraClient().IssueTypes.GetIssueTypesForProjectAsync("ED");
            var telecomIssueType = issueTypes.Where(x => x.Name == "Telecom Request").FirstOrDefault();
            var editMeta = await clientprovider.GetJiraClient().Issues.GetFieldsEditMetadataAsync("ED-5804");

            //var screenscheme = await clientprovider.GetJiraClient().Screens.

            var screens =await clientprovider.GetJiraClient().RestClient.ExecuteRequestAsync<JsonArray>(Method.GET, $"rest/api/2/screens");
            var test = screens;


           //var  projectWithIssue = await clientprovider.GetJiraClient().RestClient.ExecuteRequestAsync<JObject>(Method.GET, $"rest/api/2/issue/createmeta/issuetypes/{telecomIssueType.Id}?projectKeys=ED&expand=projects.issuetypes.fields");
        }
    }
}
