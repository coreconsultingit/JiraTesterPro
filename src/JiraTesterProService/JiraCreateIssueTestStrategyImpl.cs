using Atlassian.Jira;
using Microsoft.Extensions.Logging;

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
        try
        {
            
            var projectDetails = await jiraClient.Projects.GetProjectAsync(jiraTestMasterDto.Project);
            var issueType = await projectDetails.GetIssueTypesAsync();

            var type = issueType.Where(x => x.Name == "Initial Release").FirstOrDefault();
            var issueCreated = jiraClient.CreateIssue(jiraTestMasterDto.Project);
            issueCreated.Summary = jiraTestMasterDto.Summary;
            issueCreated.Type = type;
            issueCreated=await issueCreated.SaveChangesAsync();

            logger.LogInformation("{issueCreated}",issueCreated);


            return new JiraTestResult()
            {
                JiraTestMasterDto = jiraTestMasterDto,
                HasException = issueCreated.Status.Name==jiraTestMasterDto.ExpectedStatus,
                TestPassed=true

            };
        }
        catch (Exception e)
        {
            return new JiraTestResult()
            {
                JiraTestMasterDto = jiraTestMasterDto,
                HasException = true,
                ExceptionMessage = e.Message,
                TestPassed = jiraTestMasterDto.Expectation=="Failed"
            };
        }
       


    }
}