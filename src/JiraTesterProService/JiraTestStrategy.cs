using JiraTesterProData;
using JiraTesterProData.Extensions;
using JiraTesterProService.ImageHandler;
using JiraTesterProService.JiraParser;

namespace JiraTesterProService
{
    public abstract class JiraTestStrategy
    {
        protected IDictionary<string, Project> dictProject = new Dictionary<string, Project>();
        protected IDictionary<string, IEnumerable<IssueType>> dictIssueType = new Dictionary<string, IEnumerable<IssueType>>();
        protected  IJiraClientProvider jiraClientProvider;
        protected IScreenCaptureService screenCaptureService;
        protected JiraFileConfigProvider jiraFileConfigProvider;
        protected JiraTestStrategy(IJiraClientProvider jiraClientProvider,
            JiraFileConfigProvider jiraFileConfigProvider, IScreenCaptureService screenCaptureService,
            ILogger<JiraCreateIssueTestStrategyImpl> logger)
        {
            this.jiraClientProvider = jiraClientProvider;
            this.screenCaptureService = screenCaptureService;
            this.jiraFileConfigProvider = jiraFileConfigProvider;
        }

        protected async Task InitializeDictionary(JiraTestMasterDto jiraTestMasterDto, JiraTestResult jiraTestResult)
        {
            Project projectDetails;
            if (!dictProject.ContainsKey(jiraTestMasterDto.Project))
            {
                projectDetails = await jiraClientProvider.GetJiraClient().Projects.GetProjectAsync(jiraTestMasterDto.Project);
                dictProject.Add(jiraTestMasterDto.Project, projectDetails);
                jiraTestResult.ProjectName = projectDetails.Name.ToString();
                if (!dictIssueType.ContainsKey(jiraTestMasterDto.Project))
                {
                    dictIssueType.Add(jiraTestMasterDto.Project, await dictProject[jiraTestMasterDto.Project].GetIssueTypesAsync());
                }
            }
        }


        protected async Task TakeScreenShotAfterAction(JiraTestResult result)
        {
            var imagePath = Path.Combine(jiraFileConfigProvider.OutputJiraTestFilePathWithMaster,
                $"{result.JiraTestMasterDto.StepId}_{result.JiraTestMasterDto.Project}_{result.JiraTestMasterDto.Scenario}.png");
            
            await screenCaptureService.CaptureScreenShot(new ScreenShotInputDto()
            {
                FilePath = imagePath,
                TestUrl = result.JiraIssueUrl
            });
            result.ScreenShotPath = imagePath;

        }

        public abstract Task<JiraTestResult> Execute(JiraTestMasterDto jiraTestMasterDto);

        protected async Task AssertSubTaskCount(Issue issue, JiraTestMasterDto jiraTestMasterDto)
        {
            if (jiraTestMasterDto.ExpectedSubTaskCount.HasValue && jiraTestMasterDto.ExpectedSubTaskCount.Value > 0)
            {
                var subTasks = await issue.GetSubTasksAsync();
                if (subTasks.TotalItems != jiraTestMasterDto.ExpectedSubTaskCount.Value)
                {
                    throw new Exception("Sub task count doesn't match");
                }

                if (!string.IsNullOrEmpty(jiraTestMasterDto.ExpectedSubTaskList))
                {
                    if (!subTasks.Select(x => x.Summary).ToList().IsListEqual(jiraTestMasterDto.ExpectedSubTaskList.Split(",")))
                    {
                        throw new Exception("Sub task created summary doesn't match");
                    }
                }
               

            }
        }

        protected void SetJiraIssueUrl(JiraTestResult jiraTestResult,string jiraurl)
        {
            jiraTestResult.JiraIssueUrl = $"{jiraurl}browse/{jiraTestResult.JiraIssue.Key}";
        }
        protected  void AssertExpectedStatus(Issue issue, JiraTestMasterDto jiraTestMasterDto, JiraTestResult jiraTestResult)
        {
            jiraTestResult.HasException = issue.Status.Name == jiraTestMasterDto.ExpectedStatus;
        }
    }
}
