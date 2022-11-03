using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace JiraTesterProService;

public class JiraTestResultWriter : IJiraTestResultWriter
{
    public void WriteTestResult(IList<JiraTestResult> lstJiraTestResult, string filepath)
    {
        if (File.Exists(filepath))
        {
            File.Delete(filepath);
        }
        var builder = new HtmlContentBuilder();
        builder.AppendFormat("<html><head><style> table {{margin: 0 auto;}}</style> <link rel=\"stylesheet\" href=\"https://cdn.jsdelivr.net/npm/bootstrap@4.3.1/dist/css/bootstrap.min.css\" integrity=\"sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T\" crossorigin=\"anonymous\">" +
                             "</head><body>");

        builder.AppendFormat("<div class=\"container-fluid\"> <ul class=\"nav\" Style=\"background-image: linear-gradient(180deg, #4b0f41, grey); color:#fff\"><li class=\"nav-item\">" +
                             "TestResults run at :{0} {1}</li></ul>",DateTime.Now.ToLongDateString(), DateTime.Now.ToShortTimeString());
        
        
        
        
        
        builder.AppendFormat("<table border=\"1\"><tr>");

        builder.AppendFormat($"<th>GroupKey</th>");
        builder.AppendFormat($"<th>Project</th>");
        builder.AppendFormat($"<th>Summary</th>");
        builder.AppendFormat($"<th>IssueType</th>");
        builder.AppendFormat($"<th>Action</th>");
        builder.AppendFormat($"<th>Status</th>");
        builder.AppendFormat($"<th>IsSubTask</th>");
        builder.AppendFormat($"<th>TestPassed</th>");
        builder.AppendFormat($"<th>HasException</th>");
        builder.AppendFormat($"<th>Jiraref</th>");
        builder.AppendFormat($"<th>ExceptionMessage</th>");

        builder.AppendFormat("</tr>");

        foreach (var result in lstJiraTestResult)
        {
            builder.AppendFormat($"<tr bgcolor=\"{(result.TestPassed ? "green":"red")}\">");
                
            builder.AppendFormat("<td>{0}</td>", result.JiraTestMasterDto.GroupKey);
            builder.AppendFormat("<td>{0}</td>", result.JiraTestMasterDto.Project);
            builder.AppendFormat("<td>{0}</td>", result.JiraTestMasterDto.Summary);
            builder.AppendFormat("<td>{0}</td>", result.JiraTestMasterDto.IssueType);
            builder.AppendFormat("<td>{0}</td>", result.JiraTestMasterDto.Action);
            builder.AppendFormat("<td>{0}</td>", result.JiraTestMasterDto.Status);
            builder.AppendFormat("<td>{0}</td>", result.JiraTestMasterDto.IsSubTask);
            builder.AppendFormat("<td>{0}</td>", result.TestPassed);
            builder.AppendFormat("<td>{0}</td>", result.HasException);
            builder.AppendFormat("<td>{0}</td>",(result.JiraIssue==null? String.Empty: result.JiraIssue.Key.ToString()));
            builder.AppendFormat("<td>{0}</td>", result.ExceptionMessage.GetEmptyIfEmptyOrNull());

            builder.AppendFormat("</tr>");
        }

        builder.AppendFormat("</table></div></body></html>");
        using TextWriter writer = File.CreateText(filepath);
        builder.WriteTo(writer,HtmlEncoder.Default);
    }
}