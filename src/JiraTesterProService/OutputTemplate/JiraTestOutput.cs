using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;

namespace JiraTesterProService.OutputTemplate
{
    public  static class JiraTestOutput
    {
        public static string GetJiraOutPutTemplate(IList<JiraTestResult> lstTestResult, object data)
        {

            string head = HeadHtml();



            string source =
                @"";
            var template = Handlebars.Compile($"<!DOCTYPE html><html languate=\"en\">{head}<div class=\"container-fluid\"><body> " +
                                              $"{GetHeaderSections()}{source} </div></body></html>");





            return template(data);

           


        }

        public static string GetHeaderSections()
        {
            return @"<div  class=""row gx-5"">
<div class=""col-sm border rounded"">
    <table class=""table""><caption>System Summary</caption><thead><tr><th>Test</th></tr> </thead><tbody></tbody></table>
</div>

<div class=""col-sm border rounded"">
    <table class=""table""><caption>Execution Details</caption><thead><tr><th>Test</th></tr> </thead><tbody></tbody></table>
</div>

<div class=""col-sm border rounded"">
    <table class=""table""><caption>Test Automation Details</caption><thead><tr><th>Test</th></tr> </thead><tbody></tbody></table>
</div>
</div>";
        }


        public static string HeadHtml()
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
