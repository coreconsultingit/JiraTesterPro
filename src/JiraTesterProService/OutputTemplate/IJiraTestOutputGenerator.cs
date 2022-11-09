namespace JiraTesterProService.OutputTemplate;

public interface IJiraTestOutputGenerator
{
    Task<string> GetJiraOutPutTemplate(IList<JiraTestResult> lstTestResult);
}