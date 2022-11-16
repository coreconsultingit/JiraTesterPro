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
    }
}
