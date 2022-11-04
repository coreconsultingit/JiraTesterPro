using System.Collections.Generic;
using System.Text.Json;
using Atlassian.Jira;
using JiraTesterProData;
using JiraTesterProService.ImageHandler;
using JiraTesterProService.JiraParser;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace JiraTesterProService;

public class JiraCreateIssueTestStrategyImpl : JiraTestStrategy
{
   
    private ILogger<JiraCreateIssueTestStrategyImpl> logger;
   
    public JiraCreateIssueTestStrategyImpl(IJiraClientProvider jiraClientProvider, ILogger<JiraCreateIssueTestStrategyImpl> logger, JiraFileConfigProvider fileConfigProvider, IScreenCaptureService screenCaptureService):base(jiraClientProvider, fileConfigProvider, screenCaptureService,logger)
    {
        this.jiraClientProvider = jiraClientProvider;
        this.logger = logger;
        
    }

    public override async Task<JiraTestResult> Execute(JiraTestMasterDto jiraTestMasterDto)
    {
        var jiraClient = jiraClientProvider.GetJiraClient();

        //var wf = await jiraClient.RestClient.ExecuteRequestAsync<JObject>(Method.GET, "rest/api/2/issue/createmeta?projectKeys=ECOA&expand=projects.issuetypes.fields");
        //var test = wf.ToString();
        var jiraTestResult = new JiraTestResult()
        {
            JiraTestMasterDto = jiraTestMasterDto
        };
        try
        {
            logger.LogInformation("Started creating jira with the dto {@jiraTestMasterDto}", jiraTestMasterDto);
            await InitializeDictionary(jiraTestMasterDto, jiraTestResult);
             var issueType = dictIssueType[jiraTestMasterDto.Project];
            var type = issueType.Where(x => x.Name.EqualsWithIgnoreCase(jiraTestMasterDto.IssueType)).FirstOrDefault();
            var issueCreated = jiraClient.CreateIssue(jiraTestMasterDto.Project);
            issueCreated.Summary = jiraTestMasterDto.Summary;
            issueCreated.Type = type;
            
            if (!string.IsNullOrEmpty(jiraTestMasterDto.Component))
            {
                issueCreated.Components.Add(jiraTestMasterDto.Component);
            }

            if (!string.IsNullOrEmpty(jiraTestMasterDto.CustomFieldInput))
            {
             
                foreach (var field in jiraTestMasterDto.CustomFieldInput.Split("|"))
                {

                    var arrfield = field.Split(":");
                    issueCreated.CustomFields.Add(arrfield[0], arrfield[1]);
                }
            }
            issueCreated = await issueCreated.SaveChangesAsync();
            logger.LogInformation("{@issueCreated}",issueCreated);

            await AssertSubTaskCount(issueCreated, jiraTestMasterDto);
            AssertExpectedStatus(issueCreated, jiraTestMasterDto, jiraTestResult);
           
            jiraTestResult.TestPassed = true;
            jiraTestResult.JiraIssue = issueCreated;
            SetJiraIssueUrl(jiraTestResult, jiraClient.Url);

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