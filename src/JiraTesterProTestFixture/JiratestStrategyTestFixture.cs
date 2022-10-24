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
        public async Task IsAble_To_CreateIssue_If_PassedArgumentsCorrectly()
        {
            var testResult = await testStartegyFactory.GetJiraTestStrategyResult(new JiraTestMasterDto()
            {
                Project = "CUS", IsSubTask = false, IssueType = "Initial Release", Action = "Create", Summary = "Test Summary"
            });

            Assert.IsFalse(testResult.HasException);
        }

        [Test]
        public async Task IsNotAble_To_CreateIssue_IfPassedArgumentsInCorrectly()
        {
            var testResult = await testStartegyFactory.GetJiraTestStrategyResult(new JiraTestMasterDto()
            {
                Project = "CUS1",
                IsSubTask = false,
                IssueType = "Initial Release",
                Action = "Create",
                
            });

            Assert.IsTrue(testResult.HasException);
        }
    }
}