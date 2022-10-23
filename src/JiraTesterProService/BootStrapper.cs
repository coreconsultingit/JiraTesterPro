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
        public static void RegisterDependency(this IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();
            services.AddSingleton(provider => configuration);
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddSingleton<JiraTestStrategy, JiraCreateIssueTestStrategyImpl>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
