using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using JiraTesterProService.ImageHandler;
using JiraTesterProService.JiraParser;
using Microsoft.Extensions.Logging;

namespace JiraTesterProService;

public class JiraUpdateIssueTestStrategyImpl: JiraTestStrategy
{
    private IJiraClientProvider jiraClientProvider;
    private ILogger<JiraCreateIssueTestStrategyImpl> logger;
    public JiraUpdateIssueTestStrategyImpl(IJiraClientProvider jiraClientProvider, ILogger<JiraCreateIssueTestStrategyImpl> logger, IJiraFileConfigProvider fileConfigProvider, IScreenCaptureService screenCaptureService) : base(jiraClientProvider, fileConfigProvider, screenCaptureService, logger)
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
            await InitializeDictionary(jiraTestMasterDto, jiraTestResult);
            logger.LogInformation("Started updating jira with the dto {@jiraTestMasterDto}", jiraTestMasterDto);
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
                var lstCustomFieldInput = issueToUpdate.CustomFields.Select(x => x.Name);
                foreach (var field in jiraTestMasterDto.CustomFieldInput.Split("|"))
                {

                    var arrfield = field.Split(":");
                    if (!lstCustomFieldInput.Contains(arrfield[0]))
                    {
                        issueToUpdate.CustomFields.Add(arrfield[0], arrfield[1]);
                    }
                }
            }

            //temp comment handling
            // var updateIssueMeta= await issueToUpdate.GetIssueFieldsEditMetadataAsync();
            //if(issueToUpdate.)
            // issueToUpdate.CustomFields.Add("Comment", "Test");
            
            await issueToUpdate.WorkflowTransitionAsync(jiraTestMasterDto
                .Status, new WorkflowTransitionUpdates() { Comment = "test" });
            //= //issuestatus.Where(x => x.Name == jiraTestMasterDto.Status);
            jiraTestResult.TestPassed = true;
            jiraTestResult.JiraIssue = issueToUpdate;
            jiraTestResult.HasException = false;
            SetJiraIssueUrl(jiraTestResult, jiraClient.Url);
            AssertExpectedStatus(issueToUpdate, jiraTestMasterDto, jiraTestResult);

        }
        catch (Exception e)
        {
            jiraTestResult.HasException = true;
            jiraTestResult.ExceptionMessage = e.Message;
            jiraTestResult.TestPassed = jiraTestMasterDto.Expectation == "Failed";

        }

        await TakeScreenShotAfterAction(jiraTestResult);

        return jiraTestResult;
    }
}