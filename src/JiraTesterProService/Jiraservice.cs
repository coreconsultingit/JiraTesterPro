using Atlassian.Jira;
using Atlassian.Jira.Remote;
using JiraTesterProData;
using Microsoft.Extensions.Configuration;

namespace JiraTesterProService;

public class Jiraservice : IJiraService
{
    private string username;
    private string passwordtoken;
    private string jiraurl;
    private Jira jiraclient;
    public Jiraservice(string username, string passwordtoken, string jiraurl)
    {
        //username = configuration.GetValue<string>("Jira:User");
        //passwordtoken = configuration.GetValue<string>("Jira:Token");
        //jiraurl = configuration.GetValue<string>("Jira:Url");
        this.username = username;
        this.passwordtoken = passwordtoken;
        this.jiraurl = jiraurl;
    }




    public async Task<Issue> CreateJira(JiraTestMasterDto dto )
    {
        var jira = GetJiraClient();


        var test = await jira.Projects.GetProjectAsync(dto.Project);
        var issueType = await test.GetIssueTypesAsync();

        var type = issueType.Where(x => x.Name == dto.IssueType).FirstOrDefault();
        var issueCreated = jira.CreateIssue(dto.Project);
        issueCreated.Summary = "test from api";
        issueCreated.Type = type;
       // issueCreated.Components.Add("Web development");

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