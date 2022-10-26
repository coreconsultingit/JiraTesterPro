using JiraTesterProData.Extensions;

namespace JiraTesterProService
{
    public abstract class JiraTestStrategy
    {
        public abstract Task<JiraTestResult> Execute(JiraTestMasterDto jiraTestMasterDto);

        protected async Task AssertSubTaskCount(Issue issue, JiraTestMasterDto jiraTestMasterDto)
        {
            if (jiraTestMasterDto.ExpectedSubTaskCount.HasValue && jiraTestMasterDto.ExpectedSubTaskCount.Value > 0)
            {
                var subTasks = await issue.GetSubTasksAsync();
                if (subTasks.TotalItems != jiraTestMasterDto.ExpectedSubTaskCount.Value)
                {
                    throw new Exception("Sub task count doesn't match");
                }

                if (!string.IsNullOrEmpty(jiraTestMasterDto.SubTaskList))
                {
                    if (subTasks.Select(x => x.Summary).ToList().IsListEqual(jiraTestMasterDto.SubTaskList.Split(",")))
                    {
                        throw new Exception("Sub task created summary doesn't match");
                    }
                }
               

            }
        }
    }
}
