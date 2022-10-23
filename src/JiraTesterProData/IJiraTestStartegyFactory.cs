namespace JiraTesterProData;

public interface IJiraTestStartegyFactory
{
    Task<JiraTestResult> GetJiraTestStrategy(JiraTestMasterDto jiraTestMasterDto);

}