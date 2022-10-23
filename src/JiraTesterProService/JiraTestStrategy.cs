namespace JiraTesterProService
{
    public abstract class JiraTestStrategy
    {
        public abstract Task<JiraTestResult> Execute(JiraTestMasterDto jiraTestMasterDto);
    }
}
