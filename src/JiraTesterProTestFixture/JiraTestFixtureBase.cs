using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraTesterProData;
using JiraTesterProService;
using JiraTesterProService.JiraParser;
using System.Reflection;
using JiraTesterProService.ImageHandler;

namespace JiraTesterProTestFixture
{
   
    public  class JiraTestFixtureBase
    {
        protected IServiceProvider _serviceProvider;
        protected string testFilePath;
        [OneTimeSetUp]
        public void BaseSetUp()
        {
            var servicecollection = new ServiceCollection();
            servicecollection.RegisterDependency(new JiraTesterCommandLineOptions());
            _serviceProvider = BootStrapper.ServiceProvider;
            var jiraFileConfigProvider = _serviceProvider.GetService<IJiraFileConfigProvider>();
            testFilePath = @"..\..\..\..\..\docs\Jira BUG Matrix.xlsx";
            jiraFileConfigProvider.InitializeConfig(new FileConfigDto()
            {
                MasterTestFile = testFilePath,
                OutputJiraTestFilePath = new FileInfo(Assembly.GetExecutingAssembly().FullName ?? @"..\").DirectoryName
            });
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
