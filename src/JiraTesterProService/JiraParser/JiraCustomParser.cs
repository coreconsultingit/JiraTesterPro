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

    public JiraCustomParser(IJiraClientProvider jiraClientProvider, ILogger<JiraCustomParser> logger)
    {
        this.jiraClientProvider = jiraClientProvider;
        this.logger = logger;
    }

    public async Task<JiraRootobject> GetParsedJiraRootBasedOnProject(string project)
    {
        try
        {
            var wf = await jiraClientProvider.GetJiraClient().RestClient.ExecuteRequestAsync<JObject>(Method.GET, "rest/api/2/workflow/rule/config");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
     

        var projectWithIssue = await jiraClientProvider.GetJiraClient().RestClient.ExecuteRequestAsync<JObject>(Method.GET, $"rest/api/2/issue/createmeta?projectKeys={project}&expand=projects.issuetypes.fields");



        var settings = new JsonSerializerSettings();
        
        var projectWithIssueString = JsonConvert.DeserializeObject<JiraRootobject>(projectWithIssue.ToString(), settings);
        var parsed = JObject.Parse(projectWithIssue.ToString());
        logger.LogInformation(projectWithIssue.ToString());

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
}