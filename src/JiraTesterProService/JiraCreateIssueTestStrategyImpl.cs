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
            var issue = jiraClient.CreateIssue(jiraTestMasterDto.Project);
            var test = await jiraClient.Projects.GetProjectAsync(jiraTestMasterDto.Project);
            var issueType = await test.GetIssueTypesAsync();

            var type = issueType.Where(x => x.Name == "Initial Release").FirstOrDefault();
            var issueCreated = jiraClient.CreateIssue(jiraTestMasterDto.Project);
            issueCreated.Summary = "test from api";
            issueCreated.Type = type;
            await issue.SaveChangesAsync();
            return new JiraTestResult()
            {
                JiraTestMasterDto = jiraTestMasterDto,
                Result = true
            };
        }
        catch (Exception e)
        {
            return new JiraTestResult()
            {
                JiraTestMasterDto = jiraTestMasterDto,
                Result = false
            };
        }
       


    }
}