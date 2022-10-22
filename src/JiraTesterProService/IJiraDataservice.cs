using Atlassian.Jira;
using JiraTesterProData;

namespace JiraTesterProService;

public interface IJiraService
{

    Jira GetJiraClient();
    Task<Issue> CreateJira(JiraTestMasterDto dto);
}