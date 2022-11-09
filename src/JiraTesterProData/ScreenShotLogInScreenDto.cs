using System.ComponentModel.DataAnnotations;

namespace JiraTesterProData;

public class ScreenShotLogInScreenDto
{
    [Required]
    
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string LoginUrl { get; set; }
   

}