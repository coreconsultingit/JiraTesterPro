using Atlassian.Jira;

namespace JiraTesterProData;

public class JiraTestResult
{
    public JiraTestMasterDto JiraTestMasterDto { get; set; }
    public bool HasException { get; set; }

  public bool TestPassed { get; set; }

    public string ExceptionMessage { get; set; }

    public Issue  JiraIssue { get; set; }
}