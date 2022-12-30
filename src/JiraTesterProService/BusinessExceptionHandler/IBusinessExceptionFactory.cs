namespace JiraTesterProService.BusinessExceptionHandler;

public interface IBusinessExceptionFactory
{
    void AddBusinessException(BusinessException businessException);

    IList<BusinessException> GetBusinessExceptionList();

    void ClearException();
}

public interface IJiraRefProvider
{
    void AddJiraRef(JiraTestMasterDto scenario, string jiraRef);
    string GetJiraRef(JiraTestMasterDto scenario);

    void Clear();

}

public class JiraRefProvider : IJiraRefProvider
{
    private IDictionary<string, string> dictjiraref = new Dictionary<string, string>();
    private IBusinessExceptionFactory businessExceptionFactory;
    private ILogger<JiraRefProvider> logger;
    public JiraRefProvider(IBusinessExceptionFactory businessExceptionFactory, ILogger<JiraRefProvider> logger)
    {
        this.businessExceptionFactory = businessExceptionFactory;
        this.logger = logger;
    }

    public void AddJiraRef(JiraTestMasterDto scenario, string jiraRef)
    {
        if (!dictjiraref.ContainsKey(scenario.GroupKey))
        {
            dictjiraref[scenario.GroupKey] = jiraRef;
        }
    }

    public string GetJiraRef(JiraTestMasterDto scenario)
    {
        if (!dictjiraref.ContainsKey(scenario.GroupKey))
        {
            string message = $"Trying to look for jira ref for scenario {scenario} but none was found for group key {scenario.GroupKey}";
            logger.LogError(message);
            businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Error, message, scenario,TestType.WorkFlow));
            return string.Empty;
        }
        else
        {
            return dictjiraref[scenario.GroupKey];
        }
       
    }

    public void Clear()
    {
        dictjiraref.Clear();
    }
}