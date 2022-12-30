using System.Diagnostics;
using JiraTesterProData;
using JiraTesterProData.Extensions;
using JiraTesterProData.JiraMapper;
using JiraTesterProService.ImageHandler;
using JiraTesterProService.JiraParser;
using PuppeteerSharp.Input;
using System.Linq;
using System.Text.RegularExpressions;
using PuppeteerSharp;
using JiraTesterProService.BusinessExceptionHandler;
using OfficeOpenXml.Drawing.Slicer.Style;

namespace JiraTesterProService
{
    public abstract class JiraTestStrategy
    {
        protected IDictionary<string, Project> dictProject = new Dictionary<string, Project>();
        protected IDictionary<string, IEnumerable<IssueType>> dictIssueType = new Dictionary<string, IEnumerable<IssueType>>();
        protected  IJiraClientProvider jiraClientProvider;
        protected IScreenCaptureService screenCaptureService;
        protected IJiraFileConfigProvider jiraFileConfigProvider;
        //protected Regex JiraRegEx = new Regex("[a-z,A-Z]+-[0-9]+", RegexOptions.Compiled);
        protected int fieldDelayInterval;
        protected ILogger<JiraTestStrategy> logger;
        private IBusinessExceptionFactory businessExceptionFactory;
        private IConfiguration configuration;
        protected int clickDelay;
        protected int taskStatusDelay;
        protected IRetryHelper retryHelper;
        protected JiraTestStrategy(IJiraClientProvider jiraClientProvider,
            IJiraFileConfigProvider jiraFileConfigProvider, IScreenCaptureService screenCaptureService,
            ILogger<JiraTestStrategy> logger, IBusinessExceptionFactory businessExceptionFactory, IConfiguration configuration, IRetryHelper retryHelper)
        {
            this.jiraClientProvider = jiraClientProvider;
            this.screenCaptureService = screenCaptureService;
            this.jiraFileConfigProvider = jiraFileConfigProvider;
            this.logger = logger;
            this.businessExceptionFactory = businessExceptionFactory;
            this.clickDelay = configuration.GetValue<int?>("ClickDelay")??200;
            this.fieldDelayInterval = configuration.GetValue<int?>("FieldDelayInterval") ?? 500;
            this.taskStatusDelay = configuration.GetValue<int?>("TaskStatusInterval") ?? 20000;
            this.retryHelper = retryHelper;
        }

       
        protected JiraTestResult GetInitializedJiraTestResult(JiraTestMasterDto jiraTestMasterDto)
        {
            return new JiraTestResult()
            {
                JiraTestMasterDto = jiraTestMasterDto,
                ProjectName = jiraTestMasterDto.ProjectName, TestPassed = false

            };
        }
        protected async Task TakeScreenShotAfterAction(JiraTestResult result)
        {
            var imagePath = Path.Combine(jiraFileConfigProvider.OutputJiraTestFilePathWithMaster, DateTime.Now.ToString("yyyyMMdd"), result.JiraTestMasterDto.GroupKey,
                $"{result.JiraTestMasterDto.StepId}_{result.JiraTestMasterDto.Project}_{result.JiraTestMasterDto.Scenario}.png");
            logger.LogInformation($"Taking screenshot for  url {result.JiraIssueUrl} path {imagePath}");
            await screenCaptureService.CaptureScreenShot(new ScreenShotInputDto()
            {
                FilePath = imagePath,
                TestUrl = result.JiraIssueUrl
            });
            result.ScreenShotPath = imagePath;

        }

        protected async Task<string> TakeScreenShotBeforeAction(JiraTestMasterDto dto, IPage page,string screenshotpart="")
        {
            var imagePath = Path.Combine(jiraFileConfigProvider.OutputJiraTestFilePathWithMaster, DateTime.Now.ToString("yyyyMMdd"), dto.GroupKey,
                $"{dto.StepId}_{dto.Project}_{dto.Scenario}_{dto.Action}_Screen{screenshotpart}.png");

            var bodyWidth = await page.EvaluateExpressionAsync<int>("document.body.scrollWidth");
            var bodyHeight = await page.EvaluateExpressionAsync<int>("document.body.scrollHeight");
            
            await page.SetViewportAsync(new ViewPortOptions()
            {
                Height = bodyHeight,
                Width = bodyWidth

            });
            var bytes = await page.ScreenshotDataAsync(new ScreenshotOptions()
            {
                FullPage = true
            });
            //logger.LogInformation($"Taking screenshot for  url {result.JiraIssueUrl} path {imagePath}");
            await screenCaptureService.CaptureScreenShot(new ScreenShotInputDto()
            {
                FilePath = imagePath,ScreenShot = bytes

            });
            return imagePath;

        }

        public abstract Task<JiraTestResult> Execute(JiraTestMasterDto jiraTestMasterDto);

        protected async Task AssertSubTaskCount(Issue issue, JiraTestMasterDto jiraTestMasterDto)
        {
            if (jiraTestMasterDto.ExpectedSubTaskCount > 0)
            {
                var subTasks = await issue.GetSubTasksAsync();
                if (subTasks.TotalItems != jiraTestMasterDto.ExpectedSubTaskCount)
                {
                    throw new Exception("Sub task count doesn't match");
                }

                if (!string.IsNullOrEmpty(jiraTestMasterDto.ExpectedSubTaskList))
                {
                    var listCompared = subTasks.Select(x => x.Summary).ToList()
                        .IsListEqual(jiraTestMasterDto.ExpectedSubTaskList.Split(","));
                    if (!listCompared.isEqual)
                    {
                        throw new Exception("Sub task created summary doesn't match");
                    }
                }
               

            }
        }

        protected void SetJiraIssueUrl(JiraTestResult jiraTestResult,string jiraurl)
        {
            if (jiraTestResult.JiraIssue!=null)
            {
                jiraTestResult.JiraIssueUrl = $"{jiraurl}browse/{jiraTestResult.JiraIssue.Key}";
            }
            else
            {
                businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Critical,"No issue key found",jiraTestResult.JiraTestMasterDto,TestType.WorkFlow));
            }
           
        }
        protected  void AssertExpectedStatus(Issue issue, JiraTestMasterDto jiraTestMasterDto, JiraTestResult jiraTestResult)
        {
            jiraTestResult.TestPassed = issue?.Status.Name == jiraTestMasterDto.ExpectedStatus;
            jiraTestResult.HasException = !jiraTestResult.TestPassed;
        }

       

        protected async Task<string> GetCreatedOrUpdatedJira(IPage page,JiraTestMasterDto dto, JiraActionEnum action)
        {
            try
            {
                logger.LogInformation($"Sleeping for {taskStatusDelay} millliseconds before proceeding for {dto.GroupKey} {dto.Status} check");
                await Task.Delay(taskStatusDelay);

                if (action.Equals(JiraActionEnum.Create))   
                {
                    
                    var jql = $"project = {dto.Project} AND summary ~ \"{dto.UniqueKey}\"";
                    
                    logger.LogInformation($"Executing the jql {jql}");                  

                        var createdJira = await jiraClientProvider.GetJiraClient().Issues.GetIssuesFromJqlAsync(jql, 5, 0);

                        if (createdJira != null && createdJira.TotalItems != 0)
                        {

                            var jirakey = createdJira.OrderByDescending(x => x.Created).First().Key.Value;

                            logger.LogInformation($"Successfully {action.ToString()} {jirakey}");
                            return jirakey;

                        }
                        throw new Exception($"No Jira found for group key {dto.UniqueKey}");
                    
                    

                }
                else
                {
                    await page.WaitForTimeoutAsync(fieldDelayInterval);
                    return dto.IssueKey;
                }
              
               
             

            }
            catch (PuppeteerException e)
            {
                logger.LogError(e.Message);
                await LogError(page, dto);
                throw;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                await LogError(page, dto);
                throw;

            }            
        }
        protected async Task LogError(IPage page, JiraTestMasterDto dto)
        {
            var errorhandler = await page.QuerySelectorAllAsync(".error");
            if (errorhandler != null && errorhandler.Any())
            {
                foreach (var handler in errorhandler)
                {
                    try
                    {
                        var error = await page.EvaluateFunctionAsync<string>("el => el.innerText", handler);
                        businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Error, error, dto, TestType.WorkFlow));
                    }
                    catch (Exception ex)
                    {
                        businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Error, ex.Message, dto, TestType.WorkFlow));
                    }

                }

            }
        }
    }
}
