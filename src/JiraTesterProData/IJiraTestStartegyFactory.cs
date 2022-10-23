namespace JiraTesterProData;

public interface IJiraTestStartegyFactory
{
    Task<JiraTestResult> GetJiraTestStrategy(JiraActionEnum jiraAction);

}