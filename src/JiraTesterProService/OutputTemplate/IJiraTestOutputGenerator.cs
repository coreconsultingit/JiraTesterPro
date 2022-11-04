namespace JiraTesterProService.OutputTemplate;

public interface IJiraTestOutputGenerator
{
    string GetJiraOutPutTemplate(IList<JiraTestResult> lstTestResult, JiraMetaDataDto metaData);
}