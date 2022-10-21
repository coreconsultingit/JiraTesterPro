using Atlassian.Jira;
using Atlassian.Jira.Remote;
using Microsoft.Extensions.Configuration;

namespace JiraTesterProService;

public class Jiraservice : IJiraService
{
    private string username;
    private string passwordtoken;
    private string jiraurl;
    private Jira jiraclient;
    public Jiraservice(IConfiguration configuration)
    {
        //username = configuration.GetValue<string>("Jira:User");
        //passwordtoken = configuration.GetValue<string>("Jira:Token");
        //jiraurl = configuration.GetValue<string>("Jira:Url");
    }




    public async Task<Issue> CreateJira(string project, Issue issue)
    {
        var jira = GetJiraClient();


        var test = await jira.Projects.GetProjectAsync(project);
        var issueType = await test.GetIssueTypesAsync();

        var type = issueType.Where(x => x.Name == "Initial Release").FirstOrDefault();
        var issueCreated = jira.CreateIssue(project);
        issueCreated.Summary = "test from api";
        issueCreated.Type = type;
        issueCreated.Components.Add("Web development");

        await issueCreated.SaveChangesAsync();
        return issueCreated;
    }

    public Jira GetJiraClient()
    {
        var settings = new JiraRestClientSettings()
        {
            EnableUserPrivacyMode = false

        };
        settings.CustomFieldSerializers["com.atlassian.jira.plugin.system.customfieldtypes:multiuserpicker"]
            = new MultiObjectCustomFieldValueSerializer("displayName");
        return Jira.CreateRestClient(jiraurl, username, passwordtoken, settings);
    }

}