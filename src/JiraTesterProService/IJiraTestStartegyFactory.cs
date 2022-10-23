namespace JiraTesterProService;

public interface IJiraTestStartegyFactory
{
    Task<JiraTestResult> GetJiraTestStrategy(JiraTestMasterDto jiraTestMasterDto);

}