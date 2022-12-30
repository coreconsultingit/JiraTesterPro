using Atlassian.Jira;
using JiraTesterProData.JiraMapper;

namespace JiraTesterProData;

public class JiraIssue
{
    public string Status { get; set; }
    public string Key { get; set; }

    public JiraIssue(string key, string status
    )
    {
        Status = status;
        Key = key;
    }
}
public class JiraTestResult
{
    public JiraTestMasterDto JiraTestMasterDto { get; set; }
    public bool HasException { get; set; }

  public bool TestPassed { get; set; }

  



    public string TestStatus
  {
      get
      {
          if (TestPassed)
          {
              return "Passed";
          }

          return "Failed";
      }
  }
  
  public string Comment { get; set; }
  public string ExceptionMessage { get; set; }
  public JiraIssue JiraIssue { get; set; }

  public string ProjectName { get; set; }

  public string JiraIssueUrl { get; set; }
  public string ScreenShotPath { get; set; }
  public string InputScreenShotPath { get; set; }

  public string InputScreenFileName
  {
      get
      {
          if (!string.IsNullOrEmpty(InputScreenShotPath))
          {
              return new FileInfo(InputScreenShotPath).Name;
            }
          return String.Empty;
      }
  }
      
      

  public string ScreenShotFileName => new FileInfo(ScreenShotPath).Name;
  public IList<ScreenTestResult> ScreenTestResult { get; set; }

  public bool FailedScreenTestResultStatus => ScreenTestResult.Any(x => !x.TestPassed);

  public string ScreenTestStatus
  {
      get
      {
        
         if (FailedScreenTestResultStatus)
          {
              return "Failed";
          }

          return "Passed";
      }
    }


  public JiraTestResult()
  {
      ScreenTestResult = new List<ScreenTestResult>();
  }
}

public class ScreenTestResult
{
    public ScreenTestDto ScreenTestDto { get; set; }
    public bool TestPassed { get; set; }
    public string TestStatus
    {
        get
        {
            if (TestPassed)
            {
                return "Passed";
            }

            return "Failed";
        }
    }
    public string Comment { get; set; }

    public JiraHtmlFieldDto HtmlFieldDto { get; set; }
    public ScreenTestResult()
    {
        HtmlFieldDto = new JiraHtmlFieldDto();
        ScreenTestDto = new ScreenTestDto();
    }

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