using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraTesterProData;
using JiraTesterProService.ExcelHandler;
using JiraTesterProService.FileHandler;
using JiraTesterProService.ImageHandler;
using JiraTesterProService.JiraParser;
using JiraTesterProService.OutputTemplate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.FormulaParsing.Logging;
using Serilog;
using LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory;


namespace JiraTesterProService
{
    public static class BootStrapper
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static void RegisterDependency(this IServiceCollection services, JiraTesterCommandLineOptions options)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();
            services.AddLogging(x => x.AddSerilog());
            services.AddScoped(provider => configuration);
            services.AddScoped<ILoggerFactory, LoggerFactory>();
            services.AddScoped(typeof(ILogger<>), typeof(Logger<>));

            services.AddScoped<JiraCreateIssueTestStrategyImpl>();
            services.AddScoped<JiraUpdateIssueTestStrategyImpl>();
            services.AddScoped<IJiraTestStartegyFactory, JiraTestStartegyFactory>();
            services.AddScoped<IJiraTestResultWriter, JiraTestResultWriter>();
            services.AddScoped<IJiraCustomParser, JiraParser.JiraCustomParser>();

            services.AddTransient(typeof(IFileGenerator<>), typeof(FileGenerator<>));
            
            services.AddScoped(typeof(IFileService<>), typeof(DelimettedFileService<>));


            services.AddScoped<IExcelReader, ExcelReader>();
            services.AddScoped<IJiraTestScenarioReader, JiraTestScenarioReader>();
            //services.AddScoped<IExcelWriter, ExcelWriter>();
            services.AddScoped<IFileFactory, FileFactory>();
            services.AddScoped<IFileHandlerFactory, FileHandlerFactory>();
            services.AddScoped<IScreenCaptureService, ScreenCaptureService>();
            services.AddScoped<IJiraTestOutputGenerator, JiraTestOutputGenerator>();

            services.AddScoped<DelimettedFileService>()
                .AddScoped<IFileService, DelimettedFileService>(s => s.GetService<DelimettedFileService>());

            services.AddScoped<ExcelFileService>()
                .AddScoped<IFileService, ExcelFileService>(s => s.GetService<ExcelFileService>());
            services.AddScoped<IPropertyBinder, PropertyBinder>();
            services.AddScoped(typeof(IDataTableParser<>), typeof(DataTableParser<>));

            RegisterJiraClientProvider(services, configuration, options);
            RegisterJiraFileConfigProvider(services, configuration, options);
            ServiceProvider = services.BuildServiceProvider();
        }

        private static void RegisterJiraClientProvider(IServiceCollection services, IConfiguration configuration, JiraTesterCommandLineOptions options)
        {
            var username = configuration.GetValue<string>("JiraConfig:userName");
            var passwordtoken = configuration.GetValue<string>("JiraConfig:password");
            var jiraurl = configuration.GetValue<string>("JiraConfig:jiraUrl");


            services.AddScoped<IJiraClientProvider, JiraClientProvider>(x=> new JiraClientProvider(options.Username??username, options.Password??passwordtoken,options.JiraUrl??jiraurl));
        }

        private static void RegisterJiraFileConfigProvider(IServiceCollection services, IConfiguration configuration,
            JiraTesterCommandLineOptions options)
        {
            var inputJiraTestFile = configuration.GetValue<string>("InputJiraTestFile");
            var outPutJiraTestFilePath = configuration.GetValue<string>("OutputJiraTestFilePath");
            var masterTestFile = configuration.GetValue<string>("MasterTestFile");


            services.AddScoped<JiraFileConfigProvider>(x => new JiraFileConfigProvider(options.OutputJiraTestFilePath ?? outPutJiraTestFilePath,
                options.MasterTestFile ?? masterTestFile, options.InputJiraTestFile ?? inputJiraTestFile,ServiceProvider.GetService<ILogger<JiraFileConfigProvider>>()));
        }
    }
}
