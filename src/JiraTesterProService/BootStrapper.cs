using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraTesterProData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.FormulaParsing.Logging;
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
            services.AddScoped(provider => configuration);
            services.AddScoped<ILoggerFactory, LoggerFactory>();
            services.AddScoped(typeof(ILogger<>), typeof(Logger<>));

            services.AddScoped<JiraTestStrategy, JiraCreateIssueTestStrategyImpl>();

            ServiceProvider = services.BuildServiceProvider();
        }

        private static void RegisterJiraClientProvider(IServiceCollection services, IConfiguration configuration, JiraTesterCommandLineOptions options)
        {
            var username = configuration.GetValue<string>("JiraConfig:userName");
            var passwordtoken = configuration.GetValue<string>("JiraConfig:password");
            var jiraurl = configuration.GetValue<string>("JiraConfig:jiraUrl");


            services.AddScoped<IJiraClientProvider, JiraClientProvider>(x=> new JiraClientProvider(options.Username??username, options.Password??passwordtoken,options.JiraUrl??jiraurl));
        }
    }
}
