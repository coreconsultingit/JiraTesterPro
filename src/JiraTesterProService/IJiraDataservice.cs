using Atlassian.Jira;

namespace JiraTesterProService;

public interface IJiraService
{

    Jira GetJiraClient();
}