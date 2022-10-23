namespace JiraTesterProData;

public class JiraTestResult
{
    public JiraTestMasterDto JiraTestMasterDto { get; set; }
    public bool Result { get; set; }

    public string ExceptionMessage { get; set; }
}