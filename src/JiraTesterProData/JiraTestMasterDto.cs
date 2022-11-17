using JiraTesterProData.JiraMapper;

namespace JiraTesterProData;

public class JiraTestMasterDto
{
    public int StepId { get; set; }

    public string FileName { get; set; }

    public string ParentIssueKey{ get; set; }

    public string Scenario { get; set; }

    public string IssueKey { get; set; }

    public string GroupKey { get; set; }
    public int OrderId { get; set; }

    public string Project { get; set; }

    public string IssueType { get; set; }

    public string Component { get; set; }

    public string Status { get; set; }

    public string CustomFieldInput { get; set; }
    public string Summary { get; set; }

    public string Action { get; set; }

    public string Expectation { get; set; }

    public string ExpectedStatus { get; set; }

    public int? ExpectedSubTaskCount { get; set; }
    public string ExpectedSubTaskList { get; set; }

    public bool IsSubTask { get; set; }

    public IList<ScreenTestDto>? ScreenTestDto { get; set; }
    public JiraFields JiraField { get; set; }

}