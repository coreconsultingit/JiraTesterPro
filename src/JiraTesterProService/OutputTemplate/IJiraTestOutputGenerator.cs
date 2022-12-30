namespace JiraTesterProService.OutputTemplate;

public interface IJiraTestOutputGenerator
{
    Task<string> GetJiraOutPutTemplate(IList<JiraTestResult> lstTestResult, DateTime startTime, bool individual=false);

    string GetJiraBusinessExceptionTemplate(string groupKey);

    Task<string> GetJiraScreenTestTemplate(IList<JiraTestResult> lstTestResult);
}