// See https://aka.ms/new-console-template for more information

using System.Collections;
using Atlassian.Jira;
using Atlassian.Jira.Remote;
using CommandLine;
using JiraTesterProData;
using Serilog;

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
        var settings = new JiraRestClientSettings
        {
            CustomFieldSerializers =
            {
                ["com.atlassian.jira.plugin.system.customfieldtypes:multiuserpicker"] = new MultiObjectCustomFieldValueSerializer("displayName")
            }
        };
        var jiraclient = Jira.CreateRestClient(opts.JiraUrl, opts.Username, opts.Password, settings);
        try
        {
            Log.Logger.Information(opts.ToString());
            
            var test = await jiraclient.Projects.GetProjectAsync("CUS");
            

           
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