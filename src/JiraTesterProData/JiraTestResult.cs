using Atlassian.Jira;
using System.Data;

namespace JiraTesterProData;

public class JiraTestResult
{
    public JiraTestMasterDto JiraTestMasterDto { get; set; }
    public bool HasException { get; set; }

  public bool TestPassed { get; set; }
  public string ScreenShot { get; set; }
  public string Comment { get; set; }

    public string ExceptionMessage { get; set; }

    public Issue?  JiraIssue { get; set; }
}
public class ReconConfig
{
   
    public int ReconConfigId { get; set; }
    public string ReconGroup { get; set; }
    public string TableName { get; set; }
    public string KeyColumns { get; set; }
    public string IgnoreRecColumns { get; set; }
    public string CompareColumns { get; set; }
    public int HeaderRowToIgnore { get; set; }
    public int FooterRowToIgnore { get; set; }
    public string ReplaceCharacterFromFileName { get; set; }
    public bool ConvertDateTimeToIsoDate { get; set; }




}
public class FileResult
{
    public string FileName { get; set; }
    public DataTable Table { get; set; }

    public FileResult(string fileName, DataTable table)
    {
        FileName = fileName;
        Table = table;
    }
}