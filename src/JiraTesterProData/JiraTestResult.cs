namespace JiraTesterProData;

public class JiraTestResult
{
    public JiraTestMasterDto JiraTestMasterDto { get; set; }
    public bool HasException { get; set; }

  

    public string ExceptionMessage { get; set; }
}