namespace JiraTesterProData;

public class JiraTestStartegyFactory : IJiraTestStartegyFactory
{
    private IServiceProvider serviceProvider;

    public JiraTestStartegyFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public Task<JiraTestResult> GetJiraTestStrategy(JiraActionEnum jiraAction)
    {
        switch (jiraAction)
        {
            case JiraActionEnum.Create:
                return ((JiraTestStrategy)serviceProvider.GetService(typeof(JiraCreateIssueTestStrategyImpl))).Execute();
            default:
                throw new NotImplementedException();


        }
    }
}