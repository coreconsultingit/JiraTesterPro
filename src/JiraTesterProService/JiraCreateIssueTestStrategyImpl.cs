using Atlassian.Jira;

namespace JiraTesterProService;

public class JiraCreateIssueTestStrategyImpl : JiraTestStrategy
{
    private IJiraClientProvider jiraClientProvider;

    public JiraCreateIssueTestStrategyImpl(IJiraClientProvider jiraClientProvider)
    {
        this.jiraClientProvider = jiraClientProvider;
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
            await issueCreated.SaveChangesAsync();
            return new JiraTestResult()
            {
                JiraTestMasterDto = jiraTestMasterDto,
                HasException = false,
              
            };
        }
        catch (Exception e)
        {
            return new JiraTestResult()
            {
                JiraTestMasterDto = jiraTestMasterDto,
                HasException = true,
                ExceptionMessage = e.Message
            };
        }
       


    }
}