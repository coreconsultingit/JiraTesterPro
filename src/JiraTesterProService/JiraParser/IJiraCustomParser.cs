using JiraTesterProData.JiraMapper;

namespace JiraTesterProService.JiraParser;

public interface IJiraCustomParser
{
    Task<JiraRootobject> GetParsedJiraRootBasedOnProject(string project);
}