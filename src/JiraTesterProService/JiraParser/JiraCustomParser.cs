using JiraTesterProData.JiraMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace JiraTesterProService.JiraParser;

public class JiraCustomParser : IJiraCustomParser
{
    private IJiraClientProvider jiraClientProvider;
    private ILogger<JiraCustomParser> logger;
    private IUserCredentialProvider userCredentialProvider;
    public JiraCustomParser(IJiraClientProvider jiraClientProvider, ILogger<JiraCustomParser> logger, IUserCredentialProvider userCredentialProvider)
    {
        this.jiraClientProvider = jiraClientProvider;
        this.logger = logger;
        this.userCredentialProvider = userCredentialProvider;
    }
    //Gettingtransitions
    //https://jiradev.ert.com/jira/rest/api/2/issue/ED-4810/transitions?expand=transitions.fields
    public async Task<JiraRootobject> GetParsedJiraRootBasedOnProject(string project)
    {var
            projectWithIssue = await jiraClientProvider.GetJiraClient().RestClient.ExecuteRequestAsync<JObject>(Method.GET, $"rest/api/2/issue/createmeta?projectKeys={project}&expand=projects.issuetypes.fields");
        var settings = new JsonSerializerSettings();
        
        var projectWithIssueString = JsonConvert.DeserializeObject<JiraRootobject>(projectWithIssue.ToString(), settings);
        var parsed = JObject.Parse(projectWithIssue.ToString());
        //logger.LogInformation(projectWithIssue.ToString());

        var token = parsed.Descendants()
                .OfType<JProperty>()
                .Where(p => p.Name.ToString().StartsWith("customfield"));


        for (int i = 0; i < projectWithIssueString.projects.Length; i++)
        {
            var issu = projectWithIssueString.projects[i];
            for (int j = 0; j < issu.issuetypes.Length; j++)
            {
                foreach (var toekn in token)
                {
                    var pathtofind = $"projects[{i}].issuetypes[{j}]";
                    if (toekn.Path.StartsWith(pathtofind))
                    {

                        var test = parsed.SelectToken(toekn.Path);
                        var tes1 = JsonConvert.DeserializeObject<Customfield>(test.ToString(), settings);
                        projectWithIssueString.projects[i].issuetypes[j].fields.Customfield.Add(tes1);
                        
                    }

                }
            }
        }

        return projectWithIssueString;


    }


    public async Task<JiraFields> GetParsedJiraFieldsBasedOnIssueKey(string issueKey)
    {

        var fieldWithIssue = await jiraClientProvider.GetJiraClient().RestClient.ExecuteRequestAsync<JObject>(Method.GET, $"rest/api/2/issue/{issueKey}/editmeta?&expand=projects.issuetypes.fields");
        var settings = new JsonSerializerSettings();

        var projectWithIssueString = JsonConvert.DeserializeObject<JiraFieldRootobject>(fieldWithIssue.ToString(), settings);
        var parsed = JObject.Parse(fieldWithIssue.ToString());
        logger.LogInformation(fieldWithIssue.ToString());

        var token = parsed.Descendants()
            .OfType<JProperty>()
            .Where(p => p.Name.ToString().StartsWith("customfield"));


        
         
         
                foreach (var toekn in token)
                {
                    //var pathtofind = $"projects[{i}].issuetypes[{j}]";
                    //if (toekn.Path.StartsWith(pathtofind))
                    //{

                        var test = parsed.SelectToken(toekn.Path);
                        var tes1 = JsonConvert.DeserializeObject<Customfield>(test.ToString(), settings);
                        projectWithIssueString.fields.Customfield.Add(tes1);

                    //}

                }
            
        

        return projectWithIssueString.fields; //projectWithIssueString;
    }

    public async Task<JiraMetaDataDto> GetJiraMetaData()
    {
        var jiraClient = jiraClientProvider.GetJiraClient();
        var serverInfo = await jiraClient.ServerInfo.GetServerInfoAsync(false);
        var jiraMeta = new JiraMetaDataDto()
        {
            JiraVersion = serverInfo.Version
                .ToString(),
            JiraUrl = jiraClient.RestClient.Url,
            JiraAccount = userCredentialProvider.GetJiraCredential().UserName,

        };
        return jiraMeta;
    }
}