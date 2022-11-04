using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;

namespace JiraTesterProService.OutputTemplate
{
    public  class JiraTestOutputGenerator: IJiraTestOutputGenerator
    {
        public string GetJiraOutPutTemplate(IList<JiraTestResult> lstTestResult, JiraMetaDataDto metaData)
        {

            string head = HeadHtml();

            var assemblyversion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0";
            var injectedData = new
            {
                assemblyversion,
                TestResults = lstTestResult,
                TotalTest = lstTestResult.Count(),
                PassedTest = lstTestResult.Where(x=>x.TestPassed),
                FailedTest = lstTestResult.Where(x=>!x.TestPassed),
                JiraMetaData = metaData
            };
           
            var template = Handlebars.Compile($"<!DOCTYPE html><html languate=\"en\">{head}<div class=\"container-fluid\"><body> " +
                                              $"{GetHeaderSections()}{GetTestReportSection()} </div></body></html>");





            return template(injectedData);

           


        }

        private static string GetTestReportSection()
        {
            return @"<div  class=""row"">
<div class=""col-sm border rounded"">
    <table class=""table""><caption>Test Report</caption><thead>
<tr><th>StepId</th><th>ExecutedScenario</th><th>Action</th><th>Expected Result</th><th>Actual Result</th><th>Pass/Fail</th><th>IssueKey</th><th>ScreenShots</th><th>Comments</th>
</tr> </thead>
<tbody>
{{#each TestResults}}
<tr>
<td>{{TestPassed}}</td>
</tr>

{{/each}}
</tbody>

</table>
</div>
</div>";
        }




        private  string GetHeaderSections()
        {
            return @" <div  class=""row gx-10"">
<div class=""col-sm rounded"">
    <table class=""table""><caption>System Summary</caption><thead>
<tr><th>Jira Version</th> <th>{{JiraMetaData.JiraVersion}}</th>  </tr> 
<tr><th>Jira Url</th> <th>{{JiraMetaData.JiraUrl}}</th>  </tr>
<tr><th>TestFileName</th> <th>{{JiraMetaData.TestFileName}}</th>  </tr> 
<tr><th>JiraAccount</th> <th>{{JiraMetaData.JiraAccount}}</th>  </tr>
<tr><th>TestRunBy</th> <th>{{JiraMetaData.TestRunBy}}</th>  </tr>
</thead><tbody></tbody></table>
</div>

<div class=""col-sm rounded"">
    <table class=""table""><caption>Execution Details</caption><thead><tr><th>Total Tests</th> <th>{{TotalTest}}</th></tr> </thead><tbody><tr><td></td></tr></tbody></table>
</div>

<div class=""col-sm rounded"">
    <table class=""table""><caption>Test Automation Details</caption><tbody><tr><td>JiraTestProVersion</td><td>{{assemblyversion}}</td></tr></tbody></table>
</div>
</div>";
        }


        private string HeadHtml()
        {
            return @"<head>
<link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/bootstrap@4.3.1/dist/css/bootstrap.min.css"" integrity=""sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T"" crossorigin=""anonymous"">
<style>
        body {
          font-family: Arial;
          font-size: 12px;
          padding: 3px;
          overflow-x: hidden;
          }
caption {    
    background: #0098c3;    
    color: #FFFFFF;
    caption-side:top;
    font-size: 20px;
    font-weight: bold;
    padding-bottom: 10px;
    text-align: center;  
}

</style>
 

</head>";
        }



    }
}
