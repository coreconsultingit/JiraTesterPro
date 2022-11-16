using System.Collections.Generic;
using System.Text.Json;
using Atlassian.Jira;
using JiraTesterProData;
using JiraTesterProData.JiraMapper;
using JiraTesterProService.ImageHandler;
using JiraTesterProService.JiraParser;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace JiraTesterProService;

public class JiraCreateIssueTestStrategyImpl : JiraTestStrategy
{
   
    private ILogger<JiraCreateIssueTestStrategyImpl> logger;
    private IDictionary<string, JiraRootobject> dictProjectWithJira = new Dictionary<string, JiraRootobject>();
    private IDictionary<string, JiraIssuetype> dictProjectWithIssueTypeJira = new Dictionary<string, JiraIssuetype>();
    private IJiraCustomParser jiraCustomParser;
    public JiraCreateIssueTestStrategyImpl(IJiraClientProvider jiraClientProvider, ILogger<JiraCreateIssueTestStrategyImpl> logger, IJiraFileConfigProvider fileConfigProvider, IScreenCaptureService screenCaptureService, IJiraCustomParser jiraCustomParser):base(jiraClientProvider, fileConfigProvider, screenCaptureService,logger)
    {
        this.jiraClientProvider = jiraClientProvider;
        this.logger = logger;
        this.jiraCustomParser = jiraCustomParser;
    }

    public override async Task<JiraTestResult> Execute(JiraTestMasterDto jiraTestMasterDto)
    {
        var jiraClient = jiraClientProvider.GetJiraClient();

        //var wf = await jiraClient.RestClient.ExecuteRequestAsync<JObject>(Method.GET, "rest/api/2/issue/createmeta?projectKeys=ECOA&expand=projects.issuetypes.fields");
        //var test = wf.ToString();
        ///rest/api/3/issuetypescreenscheme/project?projectId={projectKey}
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
            await PopulateRequiredFields(jiraTestMasterDto);


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


    private async Task PopulateRequiredFields(JiraTestMasterDto dto)
    {
        var projectCodeVal = dto.Project;

        if (!dictProjectWithJira.ContainsKey(projectCodeVal))
        {
            dictProjectWithJira.Add(projectCodeVal, await jiraCustomParser.GetParsedJiraRootBasedOnProject(projectCodeVal));
        }
        var root = dictProjectWithJira[dto.Project];
        var key = $"{dto.Project}_{dto.IssueType}";
        if (!dictProjectWithIssueTypeJira.ContainsKey(key))
        {
            var field = root.projects[0].issuetypes.Where(x => x.name.EqualsWithIgnoreCase(dto.IssueType))
                .FirstOrDefault();
            if (field == null)
            {
                logger.LogError($"Issue type {dto.IssueType} not found for {dto.Project}");
                return;
            }

            dictProjectWithIssueTypeJira.Add(key, field);
        }


        //Summary
        var fielddefinition = dictProjectWithIssueTypeJira[key].fields;
        if (fielddefinition.summary.required)
        {
            dto.Summary = "Test Summary";
        }

        if (fielddefinition.components.required)
        {
            dto.Component = fielddefinition.components.allowedValues[0].name;
        }

        var customFieldinput = new List<string>();
        foreach (var customfield in fielddefinition.Customfield)
        {
            if (customfield.required && !customfield.hasDefaultValue)
            {
                if (customfield.allowedValues.Any())
                {
                    customFieldinput.Add($"{customfield.name}:{customfield.allowedValues[0].value}");
                }
                else
                {
                    customFieldinput.Add($"{customfield.name}:Test");
                }
            }
        }
        dto.CustomFieldInput = string.Join("|", customFieldinput);
    }
}