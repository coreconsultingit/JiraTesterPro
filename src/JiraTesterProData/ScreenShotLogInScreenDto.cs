using System.ComponentModel.DataAnnotations;

namespace JiraTesterProData;

public class JiraLogInDto
{
    [Required]
    
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]

    public string LoginUrl { get; set; }
   

}

public class FileConfigDto
{
    public string OutputJiraTestFilePath { get;  set; }

    public string MasterTestFile { get;  set; }

    public string InputJiraTestFile { get;  set; }
}