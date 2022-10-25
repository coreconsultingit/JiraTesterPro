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

        [Test,Ignore("Make tests project specific")]
        public async Task IsAble_To_CreateIssue_If_PassedArgumentsCorrectly()
        {
            var testResult = await testStartegyFactory.GetJiraTestStrategyResult(new JiraTestMasterDto()
            {
                Project = "CUS", IsSubTask = false, IssueType = "Initial Release", Action = "Create", Summary = "Test Summary",
                ExpectedStatus = "Created"
            });

            Assert.IsFalse(testResult.HasException);
        }

        [Test]
        public async Task IsAble_To_CreateIssues_If_PassedArgumentsCorrectly()
        {

            var jiraMasterDtos = new List<JiraTestMasterDto>();
            jiraMasterDtos.Add(new JiraTestMasterDto()
            {
                GroupKey = "1",
                OrderId = 1,
                Project = "SEC",
                IsSubTask = false,
                IssueType = "Initial Release",
                Action = "Create",
                Component = "ERT Internal",
                Summary = "Test Summary Created from automated test",
                ExpectedStatus = "Created",
                SubTaskList = "Sub Task - Resource Request-1 - Successfully created."

            });
            jiraMasterDtos.Add(new JiraTestMasterDto()
            {
                GroupKey = "1",
                OrderId = 2,
                Project = "SEC",
                IsSubTask = true,
                IssueType = "Resource Request-1",
                
                Action = "Update",
                Component = "ERT Internal",
                Summary = "Test Summary Created from automated test",
                ExpectedStatus = "Created",
                Status = "In Progress"
            });
            jiraMasterDtos.Add(new JiraTestMasterDto()
            {
                GroupKey = "1",
                OrderId = 2,
                Project = "SEC",
                IsSubTask = true,
                IssueType = "Resource Request-1",

                Action = "Update",
                Component = "ERT Internal",
                Summary = "Test Summary Created from automated test",
                ExpectedStatus = "Created",
                Status = "Completed"
            });


            var testResult = await testStartegyFactory.GetJiraTestStrategyResult(jiraMasterDtos);

            Assert.IsNotEmpty(testResult);

        }

        [Test, Ignore("Make tests project specific")]
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