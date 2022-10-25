namespace JiraTesterProService;

public interface IJiraTestStartegyFactory
{
    Task<JiraTestResult> GetJiraTestStrategyResult(JiraTestMasterDto jiraTestMasterDto);
    Task<IList<JiraTestResult>> GetJiraTestStrategyResult(IList<JiraTestMasterDto> jiraTestMasterDto);

}