using JiraTesterProData;
using Microsoft.Extensions.DependencyInjection;

namespace JiraTesterProTestFixture
{
    public class JiratestStrategyTestFixture: JiraTestFixtureBase
    {

        private IJiraTestStartegyFactory testStartegyFactory;

        [SetUp]
        public void SetUp()
        {
            testStartegyFactory = _serviceProvider.GetService<IJiraTestStartegyFactory>();
        }

        [Test]
        public void IsAble_To_CreateTest_IfPassedArgumentsCorrectly()
        {
            var testResult = testStartegyFactory.GetJiraTestStrategy()
        }
    }
}