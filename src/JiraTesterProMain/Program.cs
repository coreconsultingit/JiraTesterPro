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
            var fileFactory = serviceProvider.GetService<IFileFactory>();
            var config = serviceProvider.GetService<IConfiguration>();
            var inputJiraTestFile = opts.InputJiraTestFile ??
                                    config.GetValue<string>("InputJiraTestFile");

            var jirabugMasterFile = opts.InputJiraTestFile ??
                                    config.GetValue<string>("MasterBugFile");

            if (inputJiraTestFile == null && jirabugMasterFile == null)
            {
                Log.Logger.Error("Either one of bug or test file should be provided");
            }

            IList<JiraTestMasterDto> lstJiraTestItems = new List<JiraTestMasterDto>();
            if (jirabugMasterFile == null)
            {
                var testFileData =
                    await fileFactory.GetDataTableFromFile(new FileInfo(inputJiraTestFile));



                var parser = serviceProvider.GetService<IDataTableParser<JiraTestMasterDto>>();

                var parsedItems = parser.ConvertDataTableToList(testFileData, null);
                if (parsedItems.lstValidationMessage.Any())
                {
                    foreach (var message in parsedItems.lstValidationMessage)
                    {
                        Log.Logger.Error(message);
                    }

                    throw new Exception("Error parsing the file");
                }

                lstJiraTestItems = parsedItems.lstItems.ToList();


            }
            else
            {
                var jiraTestScenarioReader = serviceProvider.GetService<IJiraTestScenarioReader>();
                lstJiraTestItems = await jiraTestScenarioReader.GetJiraMasterDtoFromMatrix(jirabugMasterFile);
            }

            
            var testStartegyFactory = serviceProvider.GetService<IJiraTestStartegyFactory>();

            var testResult = await testStartegyFactory.GetJiraTestStrategyResult(lstJiraTestItems);

            var writer = serviceProvider.GetService<IJiraTestResultWriter>();
            writer.WriteTestResult(testResult,
                opts.OutputJiraTestFile ?? config.GetValue<string>("OutputJiraTestFile"));
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