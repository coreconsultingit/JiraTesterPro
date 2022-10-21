// See https://aka.ms/new-console-template for more information

using System.Collections;
using Atlassian.Jira;
using Atlassian.Jira.Remote;
using CommandLine;
using JiraTesterProData;

class Program
{
    static async Task Main(string[] args)
    {
        Parser.Default.ParseArguments<JiraTesterCommandLineOptions>(args)
            .WithParsed(async opts => await DoSomeWork(opts))
            .WithNotParsed((errs) => HandleParseError(errs));
    }

    static async Task DoSomeWork(JiraTesterCommandLineOptions opts)
    {
        var settings = new JiraRestClientSettings()
        {
            EnableUserPrivacyMode = false

        };
        settings.CustomFieldSerializers["com.atlassian.jira.plugin.system.customfieldtypes:multiuserpicker"]
            = new MultiObjectCustomFieldValueSerializer("displayName");
        var jiraclient = Jira.CreateRestClient(opts.JiraUrl, opts.Username, opts.Password, settings);
        try
        {
            Console.WriteLine(opts.ToString());
            var test =  jiraclient.Projects.GetProjectAsync("CUS").Result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }


        //var issues = await jiraclient.Issues.CreateIssueAsync();
    }

    static void HandleParseError(IEnumerable errs)
    {
        Console.WriteLine("Command Line parameters provided were not valid!");
    }

}




