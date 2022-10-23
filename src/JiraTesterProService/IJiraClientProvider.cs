using Atlassian.Jira;
using JiraTesterProData;

namespace JiraTesterProService
{
    public  interface IJiraClientProvider
    {
        Jira GetJiraClient(JiraTesterCommandLineOptions options);
    }
}
