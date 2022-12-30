using Microsoft.Extensions.DependencyInjection;
using JiraTesterProData;
using JiraTesterProService;
using JiraTesterProService.JiraParser;
using System.Reflection;
using JiraTesterProService.ImageHandler;
using Microsoft.Extensions.Logging;

namespace JiraTesterProTestFixture
{
   
    public  class JiraTestFixtureBase
    {
        protected IServiceProvider _serviceProvider;
        protected string testFilePath;
        protected string directoryPath;
        private ILogger<JiraTestFixtureBase> logger;
        [OneTimeSetUp]
        public async Task BaseSetUp()
        {
            
            directoryPath = new FileInfo(Assembly.GetExecutingAssembly().FullName ?? @"..\").DirectoryName;
            var servicecollection = new ServiceCollection();
            servicecollection.RegisterDependency(new JiraTesterCommandLineOptions());
            _serviceProvider = BootStrapper.ServiceProvider;
            logger = _serviceProvider.GetService<ILogger<JiraTestFixtureBase>>();
            var jiraFileConfigProvider = _serviceProvider.GetService<IJiraFileConfigProvider>();

            testFilePath = $"{directoryPath}{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}docs{Path.DirectorySeparatorChar}Jira BUG Matrix New_Automation_Latest Workflows_V1.0.xlsx";
            logger.LogInformation($"Test file path used {testFilePath}");
            logger.LogInformation($"Directory path used {directoryPath}");
            jiraFileConfigProvider.InitializeConfig(new FileConfigDto()
            {
                MasterTestFile = testFilePath,
                OutputJiraTestFilePath = directoryPath
                
            });

            var screenCaptureService = _serviceProvider.GetService<IScreenCaptureService>();
            if (screenCaptureService != null)
            {
                logger.LogInformation($"starting the login session");
                await screenCaptureService.SetStartSession();
            }
        }

        [OneTimeTearDown]
        public async Task BaseTearDown()
        {
            var screenCaptureService = _serviceProvider.GetService<IScreenCaptureService>();
            if (screenCaptureService != null)
            {
                await screenCaptureService.CloseSession();
            }

        }

    }
}
