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

                if (!string.IsNullOrEmpty(jiraTestMasterDto.ExpectedSubTaskList))
                {
                    if (!subTasks.Select(x => x.Summary).ToList().IsListEqual(jiraTestMasterDto.ExpectedSubTaskList.Split(",")))
                    {
                        throw new Exception("Sub task created summary doesn't match");
                    }
                }
               

            }
        }

        protected  void AssertExpectedStatus(Issue issue, JiraTestMasterDto jiraTestMasterDto, JiraTestResult jiraTestResult)
        {
            jiraTestResult.HasException = issue.Status.Name == jiraTestMasterDto.ExpectedStatus;
        }
    }
}
