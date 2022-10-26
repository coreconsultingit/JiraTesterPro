using System;
using System.Collections.Generic;
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
            builder.AppendFormat($"<html><table border=\"1\">");
            builder.AppendFormat("<tr>");

            builder.AppendFormat($"<th>GroupKey</th>");
            builder.AppendFormat($"<th>Project</th>");
            builder.AppendFormat($"<th>Summary</th>");
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
                builder.AppendFormat($"<td>{result.TestPassed}</td>");
                builder.AppendFormat($"<td>{result.HasException}</td>");
                //builder.AppendFormat($"<td>{(string.IsNullOrEmpty(result.ExceptionMessage)?String.Empty : result.ExceptionMessage)}</td>");

                builder.AppendFormat("</tr>");
            }

            builder.AppendFormat("</table></html>");
            using TextWriter writer = File.CreateText(filepath);
            builder.WriteTo(writer,HtmlEncoder.Default);
        }
    }
}
