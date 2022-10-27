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
            var testFileData =
                await fileFactory.GetDataTableFromFile(new FileInfo(opts.InputJiraTestFile ??
                                                                    config.GetValue<string>("InputJiraTestFile")));


            var parser = serviceProvider.GetService<IDataTableParser<JiraTestMasterDto>>();

            var parsedItems = parser.ConvertDataTableToList(testFileData, null);
            var testStartegyFactory = serviceProvider.GetService<IJiraTestStartegyFactory>();
            if (parsedItems.lstValidationMessage.Any())
            {
                foreach (var message in parsedItems.lstValidationMessage)
                {
                    Log.Logger.Error(message);
                }

                throw new Exception("Error parsing the file");
            }

            var testResult = await testStartegyFactory.GetJiraTestStrategyResult(parsedItems.lstItems);
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