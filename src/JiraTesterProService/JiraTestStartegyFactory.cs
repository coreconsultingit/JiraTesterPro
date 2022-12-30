using System.Collections.Concurrent;
using JiraTesterProData;
using JiraTesterProService.JiraParser;

namespace JiraTesterProService;

public class JiraTestStartegyFactory : IJiraTestStartegyFactory
{
    private IServiceProvider serviceProvider;
    private ILogger<JiraTestStartegyFactory> logger;
    protected ConcurrentDictionary<string, Project> dictProject = new ConcurrentDictionary<string, Project>();
    protected IJiraClientProvider jiraClientProvider;
    public JiraTestStartegyFactory(IServiceProvider serviceProvider, IJiraClientProvider jiraClientProvider, ILogger<JiraTestStartegyFactory> logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
        this.jiraClientProvider = jiraClientProvider;
    }


    public  async Task<JiraTestResult> GetJiraTestStrategyResult(JiraTestMasterDto jiraTestMasterDto)
    {
        logger.LogInformation($"Started running the test for workflow {jiraTestMasterDto.GroupKey} action {jiraTestMasterDto.Action}");
        var parsedAction = Enum.TryParse<JiraActionEnum>(jiraTestMasterDto.Action, true, out JiraActionEnum jiraAction);
        if (parsedAction)
        {
            switch (jiraAction)
            {
                case JiraActionEnum.Create:
                    return await ((JiraTestStrategy)serviceProvider.GetService(typeof(JiraCreateIssueTestStrategyImpl)))
                        .Execute(jiraTestMasterDto);
                case JiraActionEnum.Update:
                    return await ((JiraTestStrategy)serviceProvider.GetService(typeof(JiraUpdateIssueTestStrategyImpl)))
                        .Execute(jiraTestMasterDto);
                default:
                    throw new NotImplementedException();


            }
        }
        else
        {
            throw new InvalidDataException();
        }

    }

    public async Task<IList<JiraTestResult>> GetJiraTestStrategyResult(IList<JiraTestMasterDto> jiraTestMasterDto)
    {
        var orderedTests = jiraTestMasterDto.OrderBy(x => x.StepId).ToLookup(x=>x.GroupKey);


        var lstTaskResults = new List<JiraTestResult>();
        foreach (var test in orderedTests)
        {

          

            logger.LogInformation($"Started running test for group key {test.Key}");
            var testToRun = orderedTests[test.Key];

            foreach (var step  in testToRun.OrderBy(x=>x.OrderId))
            {
                if (!dictProject.ContainsKey(step.Project))
                {
                    var projectDetails = await jiraClientProvider.GetJiraClient().Projects.GetProjectAsync(step.Project);
                    dictProject.TryAdd(step.Project, projectDetails);
                }

                step.ProjectName = dictProject[step.Project].Name;
                step.SuppliedValues.Add("Project", step.ProjectName);
                step.SuppliedValues.Add("Issue Type", step.IssueType);
                var result = await GetJiraTestStrategyResult(step);
                lstTaskResults.Add(result);
            }
            logger.LogInformation($"Completed running test for group key {test.Key}");
        }

       
        return lstTaskResults;
    }
}
