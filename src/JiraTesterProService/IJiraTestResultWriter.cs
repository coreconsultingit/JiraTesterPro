using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace JiraTesterProService
{
    public interface IJiraTestResultWriter
    {
        void WriteTestResult(IList<JiraTestResult> lstJiraTestResult,string filepath);
    }

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
                                 "TestResults run at :</li></ul>");
            builder.AppendFormat("<table border=\"1\"><tr>");

            builder.AppendFormat($"<th>GroupKey</th>");
            builder.AppendFormat($"<th>Project</th>");
            builder.AppendFormat($"<th>Summary</th>");
            builder.AppendFormat($"<th>IssueType</th>");
            builder.AppendFormat($"<th>IsSubTask</th>");
            builder.AppendFormat($"<th>TestPassed</th>");
            builder.AppendFormat($"<th>HasException</th>");
            //builder.AppendFormat($"<th>ExceptionMessage</th>");

            builder.AppendFormat("</tr>");

            foreach (var result in lstJiraTestResult)
            {
                builder.AppendFormat($"<tr bgcolor=\"{(result.HasException? "red":"green")}\">");
                
                builder.AppendFormat($"<td>{result.JiraTestMasterDto.GroupKey}</td>");
                builder.AppendFormat($"<td>{result.JiraTestMasterDto.Project}</td>");
                builder.AppendFormat($"<td>{result.JiraTestMasterDto.Summary}</td>");
                builder.AppendFormat($"<td>{result.JiraTestMasterDto.IssueType}</td>");
                builder.AppendFormat($"<td>{result.JiraTestMasterDto.IsSubTask}</td>");
                builder.AppendFormat($"<td>{result.TestPassed}</td>");
                builder.AppendFormat($"<td>{result.HasException}</td>");
                //builder.AppendFormat($"<td>{(string.IsNullOrEmpty(result.ExceptionMessage)?String.Empty : result.ExceptionMessage)}</td>");

                builder.AppendFormat("</tr>");
            }

            builder.AppendFormat("</table></div></body></html>");
            using TextWriter writer = File.CreateText(filepath);
            builder.WriteTo(writer,HtmlEncoder.Default);
        }
    }
}
