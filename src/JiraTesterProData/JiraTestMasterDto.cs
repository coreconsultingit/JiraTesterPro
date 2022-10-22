namespace JiraTesterProData;

public class JiraTestMasterDto
{
    public string GroupKey { get; set; }
    public int OrderId { get; set; }

    public string Project { get; set; }

    public string IssueType { get; set; }

    public string Component { get; set; }

    public string CustomFieldInput { get; set; }
    public string Summary { get; set; }

    public string Action { get; set; }

    public string Expectation { get; set; }

    public int SubTaskCount { get; set; }
    public string SubTaskList { get; set; }

    public bool IsSubTask { get; set; }

}