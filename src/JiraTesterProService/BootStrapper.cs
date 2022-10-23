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
            services.AddScoped(provider => configuration);
            services.AddScoped<ILoggerFactory, LoggerFactory>();
            services.AddScoped(typeof(ILogger<>), typeof(Logger<>));

            services.AddScoped<JiraTestStrategy, JiraCreateIssueTestStrategyImpl>();

            services.AddScoped<IJiraClientProvider, JiraClientProvider>();
            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
