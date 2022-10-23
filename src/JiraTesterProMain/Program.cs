// See https://aka.ms/new-console-template for more information

using System.Collections;
using Atlassian.Jira;
using Atlassian.Jira.Remote;
using CommandLine;
using JiraTesterProData;
using JiraTesterProService;
using RestSharp.Authenticators;
using RestSharp;
using Serilog;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace JiraTesterProMain;

class Program
{
    static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("..\\logs\\JiraTester.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.Console()
            .CreateLogger();
        Parser.Default.ParseArguments<JiraTesterCommandLineOptions>(args)
            .WithParsed(DoSomeWork)
            .WithNotParsed(HandleParseError);
    }

    static void DoSomeWork(JiraTesterCommandLineOptions opts)
    {
        Task.WaitAll(GetJiraTestResult(opts));
    }

    static async Task GetJiraTestResult(JiraTesterCommandLineOptions opts)
    {
        var servicecollection = new ServiceCollection();
        servicecollection.RegisterDependency();


        //var client = new RestClient($"https://coreconsultingit.atlassian.net/rest/api/2/");
        //var request = new RestRequest("workflow", Method.GET);

        //client.Authenticator = new HttpBasicAuthenticator(opts.Username, opts.Password);

        //var issue = new Issue
        //{
        //    fields =
        //        new Fields
        //        {
        //            description = "Issue Description",
        //            summary = "Issue Summary",
        //            project = new Project { key = "KEY" },
        //            issuetype = new IssueType { name = "ISSUE_TYPE_NAME" }
        //        }
        //};

        // var res = client.Execute<Rootobject>(request);
        //// request.AddJsonBody(issue);
        // if (res.StatusCode == HttpStatusCode.Created)
        //     Console.WriteLine("Issue: {0} successfully created", res.Data);
        // else
        //     Console.WriteLine(res.Content);




        var settings = new JiraRestClientSettings
        {
            CustomFieldSerializers =
            {
                ["com.atlassian.jira.plugin.system.customfieldtypes:multiuserpicker"] = new MultiObjectCustomFieldValueSerializer("displayName")
            }
        };
        //var jiraclient = Jira.CreateRestClient(opts.JiraUrl, opts.Username, opts.Password, settings);
        //var wf = await jiraclient.RestClient.ExecuteRequestAsync<Value[]>(Method.GET, "rest/api/2/workflow");
        var str =
            "{\"name\": \"Workflow 1\",\"description\": \"This is a workflow used for Stories and Tasks\",\"statuses\": [" +
            "{\"id\": \"1\",\"properties\": {\"jira.issue.editable\": \"false\"}},{\"id\": \"3\"}" +
            ",{\"id\": \"4\"}],\"transitions\": [{\"name\": \"Created\",\"from\": [],\"to\": \"1\",\"type\": \"initial\"},{\"name\": " +
            "\"In progress\",\"screen\": { \"id\": \"10001\"},\"from\": [\"1\"],\"rules\": {\"postFunctions\": [{\"type\": \"AssignToCurrentUserFunction\"}],\"conditions\": {\"conditions\": [" +
            "{\"type\": \"RemoteOnlyCondition\"},{\"configuration\": {\"groups\": [\"administrators\"]},\"type\": \"UserInAnyGroupCondition\"}],\"operator\": \"AND\"}},\"to\": \"3\",\"type\": \"directed\",\"properties\": {\"custom-property\": \"custom-value\"}" +
            "},{\"name\": \"Completed\",\"rules\": {\"postFunctions\": [{\"configuration\": {\"fieldId\": \"assignee\"},\"type\": \"ClearFieldValuePostFunction\"}],\"validators\": [{\"configuration\": {\"parentStatuses\": [{\"id\": \"3\"}]},\"type\": \"ParentStatusValidator\"},{\"configuration\": {\"permissionKey\": \"ADMINISTER_PROJECTS\"},\"type\": \"PermissionValidator\"}]},\"to\": \"3\",\"type\": \"global\"}]}";

        //var wf = await jiraclient.RestClient.ExecuteRequestAsync<Value[]>(Method.POST, "rest/api/3/workflow",
        //    JObject.Parse(str));
        try
        {
            Log.Logger.Information(opts.ToString());
            var jira = new Jiraservice(opts.Username, opts.Password,opts.JiraUrl);

            await jira.CreateJira(new JiraTestMasterDto()
            {
                Project = "CUS",
                IssueType = "Initial Release"
            });



        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    static void HandleParseError(IEnumerable errs)
    {
        Console.WriteLine("Command Line parameters provided were not valid!");
    }

}