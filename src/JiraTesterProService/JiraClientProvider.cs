

namespace JiraTesterProService;

public class JiraClientProvider : IJiraClientProvider
{
    private string username;
    private string passwordtoken;
    private string jiraurl;

    public JiraClientProvider(string username, string passwordtoken, string jiraurl)
    {
        this.username = username;
        this.passwordtoken = passwordtoken;
        this.jiraurl = jiraurl;
    }

    public Jira GetJiraClient(JiraRestClientSettings? settings)
    {
        if (settings == null)
        {
            settings = new JiraRestClientSettings()
            {
                EnableRequestTrace = true

            };
        }
        
        settings.CustomFieldSerializers["com.atlassian.jira.plugin.system.customfieldtypes:multiuserpicker"]
            = new MultiObjectCustomFieldValueSerializer("displayName");
        return Jira.CreateRestClient(
            
            jiraurl, username, passwordtoken, settings);
    }
}