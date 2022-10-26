using System.Runtime.InteropServices.ComTypes;
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

    public override async Task<JiraTestResult> Execute(JiraTestMasterDto jiraTestMasterDto)
    {
        var jiraClient = jiraClientProvider.GetJiraClient();
        var jiraTestResult = new JiraTestResult()
        {
            JiraTestMasterDto = jiraTestMasterDto
        };

        try
        {
            Issue issueToUpdate = null;
            if (!string.IsNullOrEmpty(jiraTestMasterDto.IssueKey))
            {
                issueToUpdate = await jiraClient.Issues.GetIssueAsync(jiraTestMasterDto.IssueKey);
            }
            else
            {
                var parentIssue = await jiraClient.Issues.GetIssueAsync(jiraTestMasterDto.ParentIssueKey);
                var subtasks = await parentIssue.GetSubTasksAsync();
                if (subtasks.Any())
                {
                    issueToUpdate = subtasks.Where(x =>
                        x.Type == jiraTestMasterDto.IssueType).FirstOrDefault();
                }
            }
            if (!string.IsNullOrEmpty(jiraTestMasterDto.CustomFieldInput))
            {
                foreach (var field in jiraTestMasterDto.CustomFieldInput.Split(","))
                {
                    var arrfield = field.Split(":");
                    issueToUpdate.CustomFields.Add(arrfield[0], arrfield[1]);
                }
            }

            
           
            //issueToUpdate.
            await issueToUpdate.WorkflowTransitionAsync(jiraTestMasterDto
                .Status); //= //issuestatus.Where(x => x.Name == jiraTestMasterDto.Status);
            jiraTestResult.TestPassed = true;
            jiraTestResult.JiraIssue = issueToUpdate;


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