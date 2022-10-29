using System.Text.Json;
using Atlassian.Jira;
using JiraTesterProData;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace JiraTesterProService;

public class JiraCreateIssueTestStrategyImpl : JiraTestStrategy
{
    private IJiraClientProvider jiraClientProvider;
    private ILogger<JiraCreateIssueTestStrategyImpl> logger;
    public JiraCreateIssueTestStrategyImpl(IJiraClientProvider jiraClientProvider, ILogger<JiraCreateIssueTestStrategyImpl> logger)
    {
        this.jiraClientProvider = jiraClientProvider;
        this.logger = logger;
    }

    public override async Task<JiraTestResult> Execute(JiraTestMasterDto jiraTestMasterDto)
    {
        var jiraClient = jiraClientProvider.GetJiraClient();

        var wf = await jiraClient.RestClient.ExecuteRequestAsync<JObject>(Method.GET, "rest/api/2/issue/createmeta?projectKeys=ECOA&expand=projects.issuetypes.fields");
        var test = wf.ToString();
        var jiraTestResult = new JiraTestResult()
        {
            JiraTestMasterDto = jiraTestMasterDto
        };
        try
        {
            logger.LogInformation("Started creating jira with the dto {@jiraTestMasterDto}", jiraTestMasterDto);

            var projectDetails = await jiraClient.Projects.GetProjectAsync(jiraTestMasterDto.Project);
            var issueType = await projectDetails.GetIssueTypesAsync();

            var type = issueType.Where(x => x.Name == jiraTestMasterDto.IssueType).FirstOrDefault();
            var issueCreated = jiraClient.CreateIssue(jiraTestMasterDto.Project);
            issueCreated.Summary = jiraTestMasterDto.Summary;
            issueCreated.Type = type;
            
            if (!string.IsNullOrEmpty(jiraTestMasterDto.Component))
            {
                issueCreated.Components.Add(jiraTestMasterDto.Component);
            }
            issueCreated = await issueCreated.SaveChangesAsync();
            
           
            logger.LogInformation("{@issueCreated}",issueCreated);

            await AssertSubTaskCount(issueCreated, jiraTestMasterDto);
            AssertExpectedStatus(issueCreated, jiraTestMasterDto, jiraTestResult);
            jiraTestResult.TestPassed = true;
            jiraTestResult.JiraIssue = issueCreated;
            
        }
        catch (Exception e)
        {
            jiraTestResult.HasException = true;
            jiraTestResult.ExceptionMessage = e.Message;
            jiraTestResult.TestPassed = jiraTestMasterDto.Expectation == "Failed";

        }
        return jiraTestResult;



    }
}