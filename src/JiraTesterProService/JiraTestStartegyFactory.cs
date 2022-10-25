namespace JiraTesterProService;

public class JiraTestStartegyFactory : IJiraTestStartegyFactory
{
    private IServiceProvider serviceProvider;

    public JiraTestStartegyFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public  async Task<JiraTestResult> GetJiraTestStrategyResult(JiraTestMasterDto jiraTestMasterDto)
    {
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
        var orderedTests = jiraTestMasterDto.OrderBy(x => x.GroupKey).ThenBy(x => x.OrderId).ThenBy(x=>x.IssueType);

        var lstTaskResults = new List<JiraTestResult>();
        var prevIssueKey = string.Empty;
        var parentIssueKey = string.Empty;
        var jiraGroup = string.Empty;
        var issueType = string.Empty;
       
        foreach (var test in orderedTests)
        {
            if (test.IsSubTask && jiraGroup == test.GroupKey)
            {
                test.ParentIssueKey = parentIssueKey;
            }

            if (jiraGroup == test.GroupKey && test.IssueType==issueType)
            {
                test.IssueKey = prevIssueKey;
            }
            var result = await GetJiraTestStrategyResult(test);
            lstTaskResults.Add(result);
            parentIssueKey = result.JiraIssue.Key.Value;
            prevIssueKey = result.JiraIssue.Key.Value;
            jiraGroup = test.GroupKey;
        }

        return lstTaskResults;
    }
}
