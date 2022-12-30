namespace JiraTesterProData;

public class JiraWorkFlowResult
{
    public bool IsSuccessfull { get; set; }

    public IList<JiraTestResult> Results { get; set; }

    public IList<JiraTestResultWriterResult> JiraTestResultWriterResult { get; set; }
}

public class JiraTestResultWriterResult
{
    public string ScenarioName { get; set; }

    public string ZipFileList { get; set; }

    public string CssClass { get; set; }

    public string MasterOutPutFilePath { get; set; }

    public string MasterZipFile { get; set; }

    public JiraTestResultWriterResult(string scenarioName, string zipFileList, string cssClass, string masterOutPutFilePath , string masterZipFile)
    {
        ScenarioName = scenarioName;
        ZipFileList = zipFileList;
        CssClass = cssClass;
        MasterOutPutFilePath = masterOutPutFilePath;
        MasterZipFile = masterZipFile;
    }
}
