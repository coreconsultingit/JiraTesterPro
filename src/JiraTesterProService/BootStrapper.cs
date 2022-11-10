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
using JiraTesterProService.Workflow;
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
            services.AddMemoryCache();

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
            services.AddSingleton<IUserCredentialProvider, UserCredentialProvider>();
            services.AddSingleton<IJiraFileConfigProvider, JiraFileConfigProvider>();
            services.AddScoped<IJiraTestWorkflowRunner, JiraTestWorkflowRunner>();
            RegisterJiraClientProvider(services);
           
            ServiceProvider = services.BuildServiceProvider();

            if (options.IsWeb==null || !options.IsWeb.Value)
            {
                var username = configuration.GetValue<string>("JiraConfig:userName");
                var passwordtoken = configuration.GetValue<string>("JiraConfig:password");
                var jiraurl = configuration.GetValue<string>("JiraConfig:jiraUrl");

                var logindto = new JiraLogInDto()
                {
                    UserName = options.Username ?? username,
                    Password = options.Password ?? passwordtoken,
                    LoginUrl = options.JiraUrl ?? jiraurl
                };

                var usercredentialprovider = ServiceProvider.GetService<IUserCredentialProvider>();
                usercredentialprovider.AddJiraCredential(logindto);


                var inputJiraTestFile = configuration.GetValue<string>("InputJiraTestFile");
                var outPutJiraTestFilePath = configuration.GetValue<string>("OutputJiraTestFilePath");
                var masterTestFile = configuration.GetValue<string>("MasterTestFile");
                var fileConfig = new FileConfigDto()
                {
                    OutputJiraTestFilePath = options.OutputJiraTestFilePath ?? outPutJiraTestFilePath,
                    MasterTestFile = options.MasterTestFile ?? masterTestFile,
                    InputJiraTestFile = options.InputJiraTestFile ?? inputJiraTestFile
                };
                var jiraFileConfig = ServiceProvider.GetService<IJiraFileConfigProvider>();
                jiraFileConfig.InitializeConfig(fileConfig);
            }
        }

        private static void RegisterJiraClientProvider(IServiceCollection services)
        {
            
            services.AddScoped<IJiraClientProvider, JiraClientProvider>();
        }

       
    }
}
