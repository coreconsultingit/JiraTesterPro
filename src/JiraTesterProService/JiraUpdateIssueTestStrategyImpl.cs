using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using JiraTesterProData.JiraMapper;
using JiraTesterProService.BusinessExceptionHandler;
using JiraTesterProService.ImageHandler;
using JiraTesterProService.JiraHtmlHelper;
using JiraTesterProService.JiraParser;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.Drawing.Slicer.Style;
using PuppeteerSharp;
using PuppeteerSharp.Input;

namespace JiraTesterProService;

public class JiraUpdateIssueTestStrategyImpl: JiraTestStrategy
{
    private IJiraScreenTestComparer jiraScreenTestComparer;
    private ILogger<JiraCreateIssueTestStrategyImpl> logger;
    private IJiraRefProvider jiraRefProvider;
    private IScreenCaptureService screenCaptureService;
    
    private IJiraFieldInputSimulator jiraFieldInputSimulator;
    private IJiraHtmlFieldDtoFactory jiraHtmlFieldDtoFactory;
    private IBusinessExceptionFactory businessExceptionFactory;
    private HashSet<string> FailedWorkFlow = new HashSet<string>();
    public JiraUpdateIssueTestStrategyImpl(IJiraClientProvider jiraClientProvider, ILogger<JiraCreateIssueTestStrategyImpl> logger, IJiraFileConfigProvider fileConfigProvider, IScreenCaptureService screenCaptureService, IJiraCustomParser jiraClientParser, IJiraRefProvider jiraRefProvider, IJiraFieldInputSimulator jiraFieldInputSimulator, IJiraHtmlFieldDtoFactory jiraHtmlFieldDtoFactory, IBusinessExceptionFactory businessExceptionFactory, IJiraScreenTestComparer jiraScreenTestComparer, IConfiguration configuration, IRetryHelper retryHelper) : base(jiraClientProvider, fileConfigProvider, screenCaptureService, logger, businessExceptionFactory,configuration,retryHelper)
    {
        this.logger = logger;
        this.screenCaptureService = screenCaptureService;
        this.jiraRefProvider = jiraRefProvider;
        this.jiraFieldInputSimulator = jiraFieldInputSimulator;
        this.jiraHtmlFieldDtoFactory = jiraHtmlFieldDtoFactory;
        this.businessExceptionFactory = businessExceptionFactory;
        this.jiraScreenTestComparer = jiraScreenTestComparer;
        
    }

    public override async Task<JiraTestResult> Execute(JiraTestMasterDto jiraTestMasterDto)
    {
        var jiraClient = jiraClientProvider.GetJiraClient();
        var jiraTestResult = GetInitializedJiraTestResult(jiraTestMasterDto);
        try
        {
            Issue issueToUpdate;
            var jiraref = jiraRefProvider.GetJiraRef(jiraTestMasterDto) ?? String.Empty;
            issueToUpdate = await jiraClient.Issues.GetIssueAsync(jiraref);
            jiraTestResult.JiraIssue = new JiraIssue(issueToUpdate.Key.Value, issueToUpdate.Status.Name) ;
            if (!string.IsNullOrEmpty(jiraref) && issueToUpdate.Key.Value !=null)
            {
                jiraTestMasterDto.IssueKey = issueToUpdate.Key.ToString();
                var transitions = await issueToUpdate.GetAvailableActionsAsync(true);
                jiraTestResult.JiraIssue = new JiraIssue(issueToUpdate.Key.Value, issueToUpdate.Status.Name);
                if (transitions.Any())
                {
                    var updatedTransition = transitions.FirstOrDefault(x => x.Name == jiraTestMasterDto.Status);
                    if (!FailedWorkFlow.Contains(jiraTestMasterDto.UniqueKey) && updatedTransition != null)
                    {
                        
                       
                        var result=await EditJira(jiraTestMasterDto);
                        if (!string.IsNullOrEmpty(result.jiraRef))
                        {
                            issueToUpdate = await jiraClient.Issues.GetIssueAsync(result.jiraRef);
                            jiraTestResult.JiraIssue = new JiraIssue(issueToUpdate.Key.Value, issueToUpdate.Status.Name);
                            jiraTestResult.ScreenTestResult = result.lstScreenTestResult;
                            jiraTestResult.InputScreenShotPath = result.screeninputPath;

                        }
                        
                    }
                    else
                    {
                        var transitionError =
                            $"Cannot perform this transition {jiraTestMasterDto.Status} currently . Available ones are {string.Join(Environment.NewLine, transitions.Select(x => x.Name))}";
                        logger.LogError(transitionError);
                    
                        businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Error, transitionError
                            , jiraTestMasterDto, TestType.WorkFlow));
                        if(!FailedWorkFlow.Contains(jiraTestMasterDto.UniqueKey))
                        {
                            FailedWorkFlow.Add(jiraTestMasterDto.UniqueKey);
                        }
                        


                    }

                }
                else
                {
                    jiraTestResult.HasException = true;
                    businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Error,
                        $"No transitions available for this issue", jiraTestMasterDto, TestType.WorkFlow));
                }

                SetJiraIssueUrl(jiraTestResult, jiraClient.Url);
                if(!FailedWorkFlow.Contains(jiraTestMasterDto.UniqueKey))
                {
                    AssertExpectedStatus(issueToUpdate, jiraTestMasterDto, jiraTestResult);
                }
                else
                {
                    jiraTestResult.TestPassed = false;
                    jiraTestResult.HasException = true;
                }
                    
                await TakeScreenShotAfterAction(jiraTestResult);
            }
            else
            {
                logger.LogError($"Cannnot find the jira to update for groupkey {jiraTestMasterDto.GroupKey} for status {jiraTestMasterDto.Status}");
                jiraTestResult.HasException = true;
                jiraTestResult.TestPassed = false;
                businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Error,
                    $"No Issue found to update. Create issue stage would have failed", jiraTestMasterDto, TestType.WorkFlow));
                
            }


        }
        catch (Exception e)
        {
            jiraTestResult.HasException = true;
            businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Error,
                $"{e.Message}", jiraTestMasterDto, TestType.WorkFlow));
            jiraTestResult.TestPassed = jiraTestMasterDto.Expectation == "Failed";

        }

       
        return jiraTestResult;
    }


    public async Task<(string jiraRef, IList<ScreenTestResult> lstScreenTestResult, string screeninputPath)> EditJira(JiraTestMasterDto dto)
    {
        var page = await screenCaptureService.GetPage();
        string inputScreenPath= String.Empty;
        try
        {
            var lstHtmlFields = new List<JiraHtmlFieldDto>();
            IList<ScreenTestResult> lstScreenTest = new List<ScreenTestResult>();
            var jiraClient = jiraClientProvider.GetJiraClient();
           
            var issueToUpdate = await jiraClient.Issues.GetIssueAsync(dto.IssueKey);
            var urltobrowse = $"{jiraClient.Url}browse/{dto.IssueKey}";
            logger.LogInformation($"Trying to browse {urltobrowse}");
            await page.GoToAsync(urltobrowse, timeout:50000);
            //await page.WaitForNavigationAsync();
            var handler = await page.WaitForSelectorAsync("#opsbar-opsbar-transitions");
            var fields = await handler.QuerySelectorAllAsync(".aui-button.toolbar-trigger.issueaction-workflow-transition");
            bool transitionfound = false;
            var transitions = await issueToUpdate.GetAvailableActionsAsync(true);
            
           // var fieldsMeta = await issueToUpdate.GetIssueFieldsEditMetadataAsync();

            if (transitions.Any())
            {
                var updatedTransition = transitions.FirstOrDefault(x => x.Name == dto.Status);

                if (updatedTransition != null)
                {
                    foreach (var field in fields)
                    {
                        var text = await field.EvaluateFunctionAsync<string>("el => el.innerText");
                        if (text.EqualsWithIgnoreCase(dto.Status))
                        {
                            try
                            {
                                await field.ClickAsync();
                                if (updatedTransition.HasScreen)
                                {
                                    //ToDO: when actual screen navigation comes up
                                    await page.WaitForNavigationAsync();
                                }
                                else
                                {

                                    var popupscreentimeout = updatedTransition.Fields.Any() ? 10000 : 3000;
                                    try
                                    {
                                        await page.WaitForSelectorAsync(
                                           ".jira-dialog-content", new WaitForSelectorOptions()
                                           {
                                               Visible = true,
                                               Timeout = popupscreentimeout
                                           });
                                        await Task.Delay(popupscreentimeout);
                                        var formfields =
                                            await page.QuerySelectorAsync("#issue-workflow-transition");

                                        

                                        var popupfields =
                                                await formfields.QuerySelectorAllAsync(".field-group,.group");
                                        logger.LogInformation($"Started finding and Simulating the data for {dto.IssueKey} {dto.Status}");
                                        for (int i = 0; i < popupfields.Length; i++)
                                        {
                                             formfields =
                                                await page.QuerySelectorAsync("#issue-workflow-transition");
                                            var groupfields = await formfields.QuerySelectorAllAsync(".field-group,.group");
                                            if (popupfields.Length == groupfields.Length)
                                            {
                                                var gfield = groupfields[i];
                                                {
                                                    try
                                                    {

                                                        var htmlfield = await jiraHtmlFieldDtoFactory.GetJiraHtmlFieldDto(dto, page, gfield);
                                                        if (string.IsNullOrEmpty(htmlfield.FieldId))
                                                        {
                                                            logger.LogWarning($"Unable to read the field group for index {i} during transition {dto.Status}");
                                                           
                                                          
                                                        }
                                                        else
                                                        {
                                                            lstHtmlFields.Add(htmlfield);
                                                            if (!string.IsNullOrEmpty(htmlfield.FieldId))
                                                            {
                                                                
                                                                await jiraFieldInputSimulator.SimulateInput(page, gfield, htmlfield, dto);
                                                                
                                                                if (htmlfield.IsVisible)
                                                                {
                                                                    await page.WaitForTimeoutAsync(fieldDelayInterval);
                                                                }
                                                            }
                                                        }
                                                       
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Error,
                                                            $"{ex.Message}", dto, TestType.WorkFlow));
                                                    }
                                                }

                                          
                                            }
                                            else
                                            {
                                                businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Warning,
                                                    "Dynamic change in screen fields is not supported", dto, TestType.WorkFlow));
                                            }

                                            if (i % 6 == 0)
                                            {
                                                await TakeScreenShotBeforeAction(dto, page, $"{i / 6}");
                                            }
                                        }
                                        logger.LogInformation($"Completed finding and Simulating the data");
                                        
                                        lstScreenTest = jiraScreenTestComparer.CompareScreenFields(lstHtmlFields, dto);
                                        var forms =
                                            await page.QuerySelectorAsync("#issue-workflow-transition-submit");
                                        // bool isRequired1 = controlinput.EndsWith("Required");
                                        inputScreenPath = await TakeScreenShotBeforeAction(dto, page);

                                        if(lstScreenTest.Select(x=>x.HtmlFieldDto).Where(x=>x.InValidValuelist.Count>0).Any())
                                        {
                                            logger.LogError($"Invalid values found in screen");
                                            return (dto.IssueKey, lstScreenTest, inputScreenPath);
                                        }

                                        else
                                        {
                                            await forms.ClickAsync(options: new ClickOptions()
                                            {
                                                Delay = clickDelay
                                            });
                                        }
                                     

                                        
                                    }
                                    catch (Exception e)
                                    {
                                        if (updatedTransition.Fields.Any())
                                        {
                                            logger.LogError("Pop Up Screen not found");
                                        }

                                        logger.LogInformation("No pop up for this transition");
                                    }


                                    
                                }
                                
                            }
                            catch (Exception e)
                            {
                                logger.LogError(e.Message);
                            }
                            break;
                        }
                    }
                }
                return (await GetCreatedOrUpdatedJira(page, dto, JiraActionEnum.Update),lstScreenTest, inputScreenPath);
            }


        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }
        finally
        {
            await page.CloseAsync();
        }
        return (String.Empty, new List<ScreenTestResult>(),String.Empty);

    }
}