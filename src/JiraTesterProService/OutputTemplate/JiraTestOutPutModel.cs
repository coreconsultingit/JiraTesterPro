namespace JiraTesterProService.OutputTemplate;

public class JiraTestOutPutModel
{
    public string Header { get; set; }
    public string HeaderSection { get; set; }
    public ILookup<string, JiraTestResult> TestResult { get; set; }

    public bool IsIndividual { get; set; }

}


public class JiraBusinessExceptionOutPutModel
{
    public string Header { get; set; }
    public ILookup<string, BusinessException> TestResult { get; set; }
    
}
public class JiraScreenTestOutPutModel
{
    public string Header { get; set; }
    public ILookup<string, IList<ScreenTestResult>> TestResult { get; set; }

}