using System.Collections.Generic;
using System.Text.Json;
using Atlassian.Jira;
using JiraTesterProData;
using JiraTesterProData.JiraMapper;
using JiraTesterProService.BusinessExceptionHandler;
using JiraTesterProService.ImageHandler;
using JiraTesterProService.JiraHtmlHelper;
using JiraTesterProService.JiraParser;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Exceptions;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using RestSharp;

namespace JiraTesterProService;

public class JiraCreateIssueTestStrategyImpl : JiraTestStrategy
{
    private IJiraScreenTestComparer jiraScreenTestComparer;
    private ILogger<JiraCreateIssueTestStrategyImpl> logger;
    private IScreenCaptureService screenCaptureService;
   
    private IJiraFieldInputSimulator jiraFieldInputSimulator;
    private IJiraHtmlFieldDtoFactory jiraHtmlFieldDtoFactory;
    private IBusinessExceptionFactory businessExceptionFactory;
    private IJiraRefProvider jiraRefProvider;
    private IRetryHelper retryHelper;
    public JiraCreateIssueTestStrategyImpl(IJiraClientProvider jiraClientProvider, ILogger<JiraCreateIssueTestStrategyImpl> logger, IJiraFileConfigProvider fileConfigProvider, IScreenCaptureService screenCaptureService, IJiraCustomParser jiraCustomParser, IJiraFieldInputSimulator jiraFieldInputSimulator, IJiraHtmlFieldDtoFactory jiraHtmlFieldDtoFactory, IBusinessExceptionFactory businessExceptionFactory, IJiraScreenTestComparer jiraScreenTestComparer, IJiraRefProvider jiraRefProvider, IConfiguration configuration, IRetryHelper retryHelper) :base(jiraClientProvider, fileConfigProvider, screenCaptureService,logger, businessExceptionFactory,configuration, retryHelper)
    {
        this.jiraClientProvider = jiraClientProvider;
        this.logger = logger;
        this.screenCaptureService = screenCaptureService;
        this.jiraFieldInputSimulator = jiraFieldInputSimulator;
        this.jiraHtmlFieldDtoFactory = jiraHtmlFieldDtoFactory;
        this.businessExceptionFactory = businessExceptionFactory;
        this.jiraScreenTestComparer = jiraScreenTestComparer;
        this.jiraRefProvider = jiraRefProvider;
        this.retryHelper = retryHelper;
    }

    public override async Task<JiraTestResult> Execute(JiraTestMasterDto jiraTestMasterDto)
    {
        var jiraClient = jiraClientProvider.GetJiraClient();
        var jiraTestResult = GetInitializedJiraTestResult(jiraTestMasterDto);
        try
        {
            IList<string> lstValidationResult = new List<string>();
            logger.LogInformation($"Started creating jira with the scenario {jiraTestMasterDto.Scenario} action {jiraTestMasterDto.Action}" );
            
            var issueref = await GetCreatedJira(jiraTestMasterDto);

            if (!string.IsNullOrEmpty(issueref.jiraRef))
            {
                jiraRefProvider.AddJiraRef(jiraTestMasterDto,issueref.jiraRef);
                var issueCreated = await jiraClientProvider.GetJiraClient().Issues.GetIssueAsync(issueref.jiraRef);
                if (jiraTestMasterDto.SuppliedValues.Keys.Contains("WorkFlowPath"))
                {
                    var pathtoVerify = jiraTestMasterDto.SuppliedValues["WorkFlowPath"].Split(",").ToList();
                    var actions = await issueCreated.GetAvailableActionsAsync();
                    var missingpath = pathtoVerify.Except(actions.Select(x => x.Name).ToList());
                    if (missingpath.Any())
                    {
                        lstValidationResult.Add($" Missing workflow path {string.Join(",", missingpath)} ");
                    }
                }
                AssertExpectedStatus(issueCreated, jiraTestMasterDto, jiraTestResult);
                jiraTestResult.TestPassed = issueCreated.Status==jiraTestMasterDto.ExpectedStatus;
                jiraTestResult.JiraIssue = new JiraIssue(issueCreated.Key.Value,issueCreated.Status.Name);
               
                SetJiraIssueUrl(jiraTestResult, jiraClient.Url);
                
                await TakeScreenShotAfterAction(jiraTestResult);
            }
            else
            {
                logger.LogError($"Unable to create the Jira for Groupkey {jiraTestMasterDto.GroupKey}");
                jiraTestResult.HasException = true;
                jiraTestResult.TestPassed = false;
            }
            jiraTestResult.ScreenTestResult = issueref.lstScreenTestResult;
            jiraTestResult.InputScreenShotPath = issueref.screeninputPath;

        }
        catch (Exception e)
        {
            jiraTestResult.HasException = true;
            jiraTestResult.ExceptionMessage = e.Message;
            jiraTestResult.TestPassed = jiraTestMasterDto.Expectation == "Failed";

        }
        
        return jiraTestResult;



    }
    public async Task<(string jiraRef, IList<ScreenTestResult> lstScreenTestResult, string screeninputPath)> GetCreatedJira(JiraTestMasterDto dto)
    {
        string inputScreenPath = String.Empty;

        var lstHtmlFields = new List<JiraHtmlFieldDto>();
        var page = await screenCaptureService.GetPage();
        IList<ScreenTestResult> screenresults=new List<ScreenTestResult>();
        await page.GoToAsync($"{jiraClientProvider.GetJiraClient().Url}secure/Dashboard.jspa");
        await page.ClickAsync("#create_link", new ClickOptions() { Delay = fieldDelayInterval });
        try
        {
            await page.WaitForSelectorAsync("#project-field");
           
            //DO Project and issuetype first
            for (int i = 0; i <= 1; i++)
            {
                var fields = await page.QuerySelectorAllAsync(".field-group,.group");
                var field = fields[i];
                var x = await jiraHtmlFieldDtoFactory.GetJiraHtmlFieldDto(dto, page, field);
                lstHtmlFields.Add(x);
                //var jirainput = jiraFieldInputProvider.GetParsedFieldValue(x, dto);
                await jiraFieldInputSimulator.SimulateInput(page, field, x, dto);
                await page.WaitForTimeoutAsync(2000);
            }

            var totalfields = await page.QuerySelectorAllAsync(".field-group,.group");
            for (int i = 2; i < totalfields.Length; i++)
            {

                var fields = await page.QuerySelectorAllAsync(".field-group,.group");
                if (totalfields.Length == fields.Length)
                {
                    var field = fields[i];

                    try
                    {
                        var x = await jiraHtmlFieldDtoFactory.GetJiraHtmlFieldDto(dto, page, field);
                        lstHtmlFields.Add(x);
                        if (!string.IsNullOrEmpty(x.FieldId))
                        {
                            await jiraFieldInputSimulator.SimulateInput(page, field, x, dto);
                            if (x.IsVisible)
                            {
                                await page.WaitForTimeoutAsync(fieldDelayInterval);
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Warning,
                            $"{e.Message}", dto, TestType.WorkFlow));
                    }

                    if(i%6==0)
                    {
                        await TakeScreenShotBeforeAction(dto, page,$"{i/6}");
                    }
                }
                else
                {
                    businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Warning,
                        "Dynamic change in screen fields is not supported", dto, TestType.WorkFlow));
                }
            }

            try
            {

                logger.LogInformation($"Taking the screenresults before creating the issue {dto.GroupKey}");
                screenresults = jiraScreenTestComparer.CompareScreenFields(lstHtmlFields,dto);
                logger.LogInformation($"Completed Taking the screenresults before creating the issue {dto.GroupKey}");
                
                logger.LogInformation($"Taking the screenshot before creating the issue {dto.GroupKey}");
                inputScreenPath = await TakeScreenShotBeforeAction(dto, page);
                logger.LogInformation($"Completed Taking the screenshot before creating the issue {dto.GroupKey}");
                logger.LogInformation($"Waiting for submit handler for creating the issue {dto.GroupKey}");
                var handler = await page.WaitForSelectorAsync("#create-issue-submit");
                logger.LogInformation($"Completed waiting  for submit handler for creating the issue {dto.GroupKey}");
                logger.LogInformation($"focussing submit handler for creating the issue {dto.GroupKey}");
                await handler.FocusAsync();
                logger.LogInformation($"completed focussing handler for creating the issue {dto.GroupKey}");
                logger.LogInformation($"started submit for creating the issue {dto.GroupKey}");

                await page.EvaluateFunctionAsync("x=>x.click()", handler);
                //await handler.ClickAsync(options: new ClickOptions()
                //{
                //    Delay = clickDelay
                //});
                logger.LogInformation($"Completed submit for creating the issue {dto.GroupKey}");
                try
                {
                    logger.LogInformation($"Going to wait for pop up window for  {dto.GroupKey}");
                    await page.WaitForSelectorAsync("#aui-flag-container > div > div");
                    logger.LogInformation($"Completed wait for pop up window for  {dto.GroupKey}");
                }
                catch(Exception ex)
                {
                    logger.LogWarning(ex.Message);
                }
                return (await GetCreatedOrUpdatedJira(page, dto, JiraActionEnum.Create), screenresults, inputScreenPath);


            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return (String.Empty, screenresults, inputScreenPath);

            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            throw;
        }
        finally
        {
            await page.CloseAsync();
            
            //
        }
       
    }
    
}