using Microsoft.Extensions.DependencyInjection;

namespace JiraTesterProService.Workflow;

public interface IJiraTestWorkflowRunner
{
    Task<IList<JiraTestResult>> RunJiraWorkflow();
}