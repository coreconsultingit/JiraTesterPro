namespace JiraTesterProService;

public interface IJiraTestStartegyFactory
{
    Task<JiraTestResult> GetJiraTestStrategyResult(JiraTestMasterDto jiraTestMasterDto);

}