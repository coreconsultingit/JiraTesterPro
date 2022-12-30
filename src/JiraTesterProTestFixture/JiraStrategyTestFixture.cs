using JiraTesterProService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraTesterProTestFixture
{
    [TestFixture]
    public class JiraStrategyTestFixture:JiraTestFixtureBase
    {
        //[Test,Ignore("Only to be used for debugging")]
        [Test]
        public async Task Adhoc_Testing_For_SpecificJira()
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            var suppliedscreenDto = new Dictionary<string, string>(comparer)
            {
                { "Project", "oneECOA DEV (ED)" },
                { "Issue Type", "Bug" },
                {"Component/s","ERT Internal"},
                {"comment","test reopen from UI"},
                {"description","test description from UI"},
                {"source","Externals"}

                // {"Cognitive Debrief Date","2022-11-28"}
            };

            var dto = new JiraTestMasterDto()
            {
                ProjectName = "oneECOA DEV (ED)",
                Project = "ED",
                IssueType = "Bug",
                SuppliedValues = suppliedscreenDto,
                Status = "New",
                Action = "Create",
                UniqueKey = "Test CreatingOne for UpdateComment",
                IssueKey= "ED-7235"
            };


            var update = _serviceProvider.GetService<JiraUpdateIssueTestStrategyImpl>();
            dto.IssueKey = "ED-7235";
            dto.Action = JiraActionEnum.Update.ToString();
            dto.Status = "Defer";
            var updatedresult = await update.EditJira(dto);
            Assert.IsNotEmpty(updatedresult.jiraRef);
        }

        [Test]
        public async Task Is_Able_To_Create_Update_With_NoSuppliedValue()
        {
            
            var comparer = StringComparer.OrdinalIgnoreCase;
            var suppliedscreenDto = new Dictionary<string, string>(comparer)
            {
                { "Project", "oneECOA DEV (ED)" },
                { "Issue Type", "Bug" },
                {"Component/s","ERT Internal"},

                // {"Cognitive Debrief Date","2022-11-28"}
            };


            var dto = new JiraTestMasterDto()
            {
                StepId=1,
               Scenario = "Test",
                ProjectName = "oneECOA DEV (ED)",
                Project = "ED",
                IssueType = "Bug",
                SuppliedValues = suppliedscreenDto,
                Status = "New",
                Action = "Create",
                UniqueKey = DateTime.Now.GenerateGuid("Test1").ToString()
        };
            dto.GroupKey = dto.UniqueKey;

            var createstrategy = _serviceProvider.GetService<JiraCreateIssueTestStrategyImpl>();
            var result = await createstrategy.GetCreatedJira(dto);
            Assert.IsNotEmpty(result.jiraRef);

            var update = _serviceProvider.GetService<JiraUpdateIssueTestStrategyImpl>();
            dto.IssueKey = result.jiraRef;
            dto.Action = JiraActionEnum.Update.ToString();
            dto.Status = "Start Progress";
            var updatedresult = await update.EditJira(dto);
            Assert.IsNotEmpty(updatedresult.jiraRef);

            dto.Status = "Resolve Issue";
            updatedresult = await update.EditJira(dto);
            Assert.IsNotEmpty(updatedresult.jiraRef);

            dto.Status = "Reopen Issue";
            updatedresult = await update.EditJira(dto);
            Assert.IsNotEmpty(updatedresult.jiraRef);

            dto.Status = "Defer";
            updatedresult = await update.EditJira(dto);
            Assert.IsNotEmpty(updatedresult.jiraRef);

            dto.Status = "Reopen Issue";
            updatedresult = await update.EditJira(dto);
            Assert.IsNotEmpty(updatedresult.jiraRef);

            dto.Status = "Start Progress";
            updatedresult = await update.EditJira(dto);
            Assert.IsNotEmpty(updatedresult.jiraRef);

            dto.Status = "Stop Progress";
            updatedresult = await update.EditJira(dto);
            Assert.IsNotEmpty(updatedresult.jiraRef);

            dto.Status = "Start Progress";
            updatedresult = await update.EditJira(dto);
            Assert.IsNotEmpty(updatedresult.jiraRef);

            dto.Status = "Resolve Issue";
            updatedresult = await update.EditJira(dto);
            Assert.IsNotEmpty(updatedresult.jiraRef);

            dto.Status = "Close Issue";
            updatedresult = await update.EditJira(dto);
            Assert.IsNotEmpty(updatedresult.jiraRef);
        }
    }
}
