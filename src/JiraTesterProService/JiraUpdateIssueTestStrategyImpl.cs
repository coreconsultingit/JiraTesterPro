using Microsoft.Extensions.Logging;

namespace JiraTesterProService;

public class JiraUpdateIssueTestStrategyImpl: JiraTestStrategy
{
    private IJiraClientProvider jiraClientProvider;
    private ILogger<JiraCreateIssueTestStrategyImpl> logger;

    public JiraUpdateIssueTestStrategyImpl(IJiraClientProvider jiraClientProvider, ILogger<JiraCreateIssueTestStrategyImpl> logger)
    {
        this.jiraClientProvider = jiraClientProvider;
        this.logger = logger;
    }

    public async override Task<JiraTestResult> Execute(JiraTestMasterDto jiraTestMasterDto)
    {
        var jiraClient = jiraClientProvider.GetJiraClient();
        var jiraTestResult = new JiraTestResult()
        {
            JiraTestMasterDto = jiraTestMasterDto
        };

        try
        {

            
            var parentIssue = await jiraClient.Issues.GetIssueAsync(string.IsNullOrEmpty(jiraTestMasterDto.ParentIssueKey)? jiraTestMasterDto.IssueKey: jiraTestMasterDto.ParentIssueKey);

            var subtasks = await parentIssue.GetSubTasksAsync();
            if (subtasks.Any())
            {
                var subTaskToUpdate = subtasks.Where(x =>
                    x.Type == jiraTestMasterDto.IssueType).FirstOrDefault();

               
               
                await subTaskToUpdate.WorkflowTransitionAsync(jiraTestMasterDto.Status);//= //issuestatus.Where(x => x.Name == jiraTestMasterDto.Status);
                jiraTestResult.TestPassed = true;
                jiraTestResult.JiraIssue = subTaskToUpdate;

            }

            

            //var issueType = await projectDetails.GetIssueTypesAsync();

            //var type = issueType.Where(x => x.Name == "Initial Release").FirstOrDefault();
            //var issueCreated = jiraClient.CreateIssue(jiraTestMasterDto.Project);
            //issueCreated.Summary = jiraTestMasterDto.Summary;
            //issueCreated.Type = type;
            //issueCreated = await issueCreated.SaveChangesAsync();

            //logger.LogInformation("{issueCreated}", issueCreated);
            //jiraTestResult.HasException = issueCreated.Status.Name == jiraTestMasterDto.ExpectedStatus;
            jiraTestResult.TestPassed = true;
            //jiraTestResult.JiraIssue = issueCreated;

            //jiraTestResult.JiraIssue = issueCreated;

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