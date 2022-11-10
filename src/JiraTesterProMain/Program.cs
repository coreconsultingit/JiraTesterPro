// See https://aka.ms/new-console-template for more information

using System.Collections;
using CommandLine;
using JiraTesterProData;
using JiraTesterProService;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using JiraTesterProService.FileHandler;
using Microsoft.Extensions.Configuration;
using JiraTesterProService.ExcelHandler;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Finance.Implementations;
using System;
using JiraTesterProService.JiraParser;
using JiraTesterProService.Workflow;

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
        var servicecollection = new ServiceCollection().AddLogging();
        servicecollection.RegisterDependency(opts);
        try
        {
            var serviceProvider = BootStrapper.ServiceProvider;
            Log.Logger.Information(opts.ToString());
            
            var config = serviceProvider.GetService<IConfiguration>();
            var inputJiraTestFile = opts.InputJiraTestFile ??
                                    config.GetValue<string>("InputJiraTestFile");

            var jirabugMasterFile = opts.InputJiraTestFile ??
                                    config.GetValue<string>("MasterTestFile");

            if (inputJiraTestFile == null && jirabugMasterFile == null)
            {
                Log.Logger.Error("Either one of bug or test file should be provided");
            }

            var workflowRunner = serviceProvider.GetService<IJiraTestWorkflowRunner>();

            await workflowRunner.RunJiraWorkflow();

        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }

        finally
        {
            Log.Information("Shut down complete");
            Log.CloseAndFlush();

        }
    }

    static void HandleParseError(IEnumerable errs)
    {
        Console.WriteLine("Command Line parameters provided were not valid!");
    }

}