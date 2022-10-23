using JiraTesterProData;
using JiraTesterProService;
using Microsoft.Extensions.DependencyInjection;

namespace JiraTesterProTestFixture
{
    [TestFixture]
    public class JiratestStrategyTestFixture: JiraTestFixtureBase
    {

        private IJiraTestStartegyFactory testStartegyFactory;

        [SetUp]
        public void SetUp()
        {
            testStartegyFactory = _serviceProvider.GetService<IJiraTestStartegyFactory>();
        }

        [Test]
        public async Task IsAble_To_CreateTest_IfPassedArgumentsCorrectly()
        {
            var testResult = await testStartegyFactory.GetJiraTestStrategy(new JiraTestMasterDto()
            {
                Project = "CUS", IsSubTask = false, IssueType = "Initial Release", Action = "Create", Summary = "Test Summary"
            });

            Assert.IsNotNull(testResult);
        }
    }
}