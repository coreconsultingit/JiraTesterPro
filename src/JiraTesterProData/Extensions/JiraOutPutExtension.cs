using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace JiraTesterProData.Extensions
{
    public static  class JiraOutPutExtension
    {
        public static string GetJiraStatus(JiraTestResult result)
        {
            if (result != null)
            {
                if (result.JiraIssue != null)
                {
                    return result.JiraIssue.Status;
                }
            }
            return String.Empty;
        }

        public static string GetInValidValue(JiraTestResult dto)
        {
            var sbMessage = new StringBuilder();
            if (dto.ScreenTestResult != null)
            {
                var invalidControls = dto.ScreenTestResult.Select(x => x.HtmlFieldDto);
                foreach (var control in invalidControls.Where(x=>x.InValidValuelist.Any()))
                {
                    sbMessage.Append($"Invalid value found for control {control.Name}");
                    foreach (var val in control.InValidValuelist)
                    {
                        sbMessage.Append($"<pre>{val}</pre>");
                    }
                }

                //return sbMessage.ToString();

            }

            return sbMessage.Length==0 ? (dto.TestPassed? JiraTestStatusConst.Passed: JiraTestStatusConst.Failed) : sbMessage.ToString();
        }
        //public string GetJirainoutScreenFile(JiraTestResult result)
        //{
        //    if (result != null)
        //    {
        //        if (!string.IsNullOrEmpty(result.InputScreenFileName))
        //        {
        //            if (File.Exists(result.InputScreenFileName))
        //            {

        //            }
                    
        //        }
        //    }

        //    return string.Empty;
        //}
    }
}
