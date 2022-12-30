namespace JiraTesterProData;

public class BusinessException
{
    public SeverityType Severity { get; set; }

    public string Message { get; set; }

    public JiraTestMasterDto JiraMaster { get; set; }

    public TestType TestType { get; set; }

    

    public BusinessException(SeverityType severity, string message, JiraTestMasterDto jiraMaster, TestType testType)
    {
        Severity = severity;
        Message = message;
        JiraMaster = jiraMaster;
        TestType = testType;
     
    }
}