using JiraTesterProData.JiraMapper;

namespace JiraTesterProService.JiraParser;

public interface IJiraCustomParser
{
    Task<JiraRootobject> GetParsedJiraRootBasedOnProject(string project);
    Task<JiraFields> GetParsedJiraFieldsBasedOnIssueKey(string issueKey);

    Task<JiraMetaDataDto> GetJiraMetaData();
}