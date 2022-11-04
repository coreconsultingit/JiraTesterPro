﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;
using JiraTesterProService.JiraParser;

namespace JiraTesterProService.OutputTemplate
{
    public  class JiraTestOutputGenerator: IJiraTestOutputGenerator
    {
        private IJiraCustomParser jiraCustomParser;
        public JiraTestOutputGenerator(IJiraCustomParser jiraCustomParser)
        {
            this.jiraCustomParser = jiraCustomParser;
        }

        public string GetJiraOutPutTemplate(IList<JiraTestResult> lstTestResult)
        {
            string head = HeadHtml();

            var assemblyversion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0";
            var injectedData = new
            {
                assemblyversion,
                TestResults = lstTestResult,
                TotalTest = lstTestResult.Count(),
                PassedTest = lstTestResult.Where(x=>x.TestPassed).Count(),
                FailedTest = lstTestResult.Where(x=>!x.TestPassed).Count(),
                JiraMetaData = jiraCustomParser.GetJiraMetaData()

        };
           
            var template = Handlebars.Compile($"<!DOCTYPE html><html languate=\"en\">{head}<div class=\"container-fluid\"><body> " +
                                              $"{GetHeaderSections()}{GetTestReportSection()} </div></body></html>");





            return template(injectedData);

           


        }

        private static string GetTestReportSection()
        {


            return @"<div  class=""row"">
<div class=""col-sm border rounded"">
    
<div class=""table-responsive"">


<table class=""table table-hover""><caption>Test Report</caption>
<thead class=""thead-light"">
<tr>
<th>StepId</th>
<th>Project</th>
<th>IssueType</th>
<th>ExecutedScenario</th>
<th>Action</th>
<th>Expected Result</th>
<th>Actual Result</th>
<th>Pass/Fail</th>
<th>IssueKey</th>
<th>Expectation</th>
<th>ScreenShots</th>
<th>Exception</th>
<th>Comments</th>

</tr> 
</thead>
<tbody>
{{#each TestResults}}
{{#if TestPassed}}
<tr class=""table-success"">
{{else}}
<tr class=""table-danger"">
{{/if}}
<td>{{JiraTestMasterDto.StepId}}</td>
<td>{{ProjectName}}</td>
<td>{{JiraTestMasterDto.IssueType}}</td>
<td>{{JiraTestMasterDto.Scenario}}</td>
<td>{{JiraTestMasterDto.Action}}</td>
<td>{{JiraTestMasterDto.Status}}</td>
<td>{{JiraIssue.Status}}</td>
<td>{{TestPassed}}</td>
<td><a href=""{{JiraIssueUrl}}"">{{JiraIssue.Key}}</a></td>
<td>{{JiraTestMasterDto.Expectation}}</td>
<th><a href=""{{ScreenShotPath}}"">{{ScreenShotPath}}</a></th>
<th>{{Exception}}</th>
<th>{{Comment}}</th>
</tr>
{{/each}}
</tbody>

</table>
</div>
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
<tr><th>JiraAccount</th> <th>{{JiraMetaData.JiraAccount}}</th>  </tr>
<tr><th>TestRunBy</th> <th>{{JiraMetaData.TestRunBy}}</th>  </tr>
</thead><tbody></tbody></table>
</div>
<div class=""col-sm rounded"">
    <table class=""table""><caption>Execution Details</caption>
<thead>
<tr><th>Total Tests</th> <th>{{TotalTest}}</th></tr> 
<tr class=""table-success""><th>Passed Tests</th> <th>{{PassedTest}}</th></tr> 
<tr class=""table-danger""><th>Failed Tests</th> <th>{{FailedTest}}</th></tr> 
</thead>
<tbody><tr><td></td></tr></tbody></table>
</div>
<div class=""col-sm rounded"">
    <table class=""table""><caption>Test Automation Details</caption><thead><th>JiraTestProVersion</th><th>{{assemblyversion}}</th></thead><tbody><tr><td></td><td></td></tr></tbody></table>
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
