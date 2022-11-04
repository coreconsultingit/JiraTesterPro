﻿namespace JiraTesterProData;

public class ScreenShotInputDto
{
   
    public string FilePath { get; set; }
   
    public string TestUrl { get; set; }

}

public class JiraMetaDataDto
{
    public string JiraUrl { get; set; }
    public string JiraVersion { get; set; }
    public string TestFileName { get; set; }

    public string JiraAccount { get; set; }

    public string TestRunBy { get; set; }

    public JiraMetaDataDto()
    {
        TestRunBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
    }
}