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
                default:
                    throw new NotImplementedException();


            }
        }
        else
        {
            throw new InvalidDataException();
        }

    }
}
