

namespace JiraTesterProService;

public class JiraClientProvider : IJiraClientProvider
{
    private string username;
    private string passwordtoken;
    private string jiraurl;

    public JiraClientProvider(IConfiguration configuration)
    {
        username = configuration.GetValue<string>("JiraConfig:userName");
        passwordtoken = configuration.GetValue<string>("JiraConfig:password");
        jiraurl = configuration.GetValue<string>("JiraConfig:jiraUrl");
    }

    public Jira GetJiraClient(JiraTesterCommandLineOptions options)
    {
        var settings = new JiraRestClientSettings()
        {
            EnableRequestTrace = true

        };
        settings.CustomFieldSerializers["com.atlassian.jira.plugin.system.customfieldtypes:multiuserpicker"]
            = new MultiObjectCustomFieldValueSerializer("displayName");
        return Jira.CreateRestClient(
            
           options.JiraUrl?? jiraurl, options.Username??username, options.Password??passwordtoken, settings);
    }
}