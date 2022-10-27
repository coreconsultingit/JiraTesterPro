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
using JiraTesterProService.FileHandler;
using System;
using Microsoft.Extensions.Configuration;

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
            .WithParsed(RunJiraTest)
            .WithNotParsed(HandleParseError);
    }

    static void RunJiraTest(JiraTesterCommandLineOptions opts)
    {
        Task.WaitAll(GetJiraTestResult(opts));
    }

    static async Task GetJiraTestResult(JiraTesterCommandLineOptions opts)
    {   
        var servicecollection = new ServiceCollection();
        servicecollection.RegisterDependency(opts);
        try
        {
            Log.Logger.Information(opts.ToString());
            var fileFactory = BootStrapper.ServiceProvider.GetService<IFileFactory>();
            var config = BootStrapper.ServiceProvider.GetService<IConfiguration>();
            var testFileData = await fileFactory.GetDataTableFromFile(new FileInfo(opts.InputJiraTestFile?? config.GetValue<string>("InputJiraTestFile")));


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