using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using JiraTesterProService.BusinessExceptionHandler;
using JiraTesterProService.JiraParser;

namespace JiraTesterProService.OutputTemplate
{
    public partial class JiraTestOutput
    {
        public JiraTestOutPutModel Model { get; set; }
    }

    public partial class JiraBusinessExceptionOutput
    {
        public JiraBusinessExceptionOutPutModel Model { get; set; }
    }

    public partial class JiraScreenTestOutput
    {
        public JiraScreenTestOutPutModel Model { get; set; }
    }

    public  class JiraTestOutputGenerator: IJiraTestOutputGenerator
    {
        private IBusinessExceptionFactory businessExceptionFactory;
        private IJiraCustomParser jiraCustomParser;
        private ILogger<JiraTestOutputGenerator> logger;
        public JiraTestOutputGenerator(IJiraCustomParser jiraCustomParser, IBusinessExceptionFactory businessExceptionFactory, ILogger<JiraTestOutputGenerator> logger)
        {
            this.jiraCustomParser = jiraCustomParser;
            this.businessExceptionFactory = businessExceptionFactory;
            this.logger = logger;
        }

        public async Task<string> GetJiraOutPutTemplate(IList<JiraTestResult> lstTestResult, DateTime startTime,bool isIndividual=false)
        {
            try
            {
                string head = HeadHtml();
                var metaData = await jiraCustomParser.GetJiraMetaData();
                var assemblyversion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0";


                var output = new JiraTestOutput();
                output.Model = new JiraTestOutPutModel()
                {
                    Header = head,
                    HeaderSection = $"\"<div class=\"row gx-10\">\" {GetHeaderSection1(metaData)}{GetHeaderSection2(lstTestResult)} {GetHeaderSection3(assemblyversion,startTime)}</div>",
                    TestResult = lstTestResult.OrderBy(x => x.JiraTestMasterDto.StepId).ToLookup(x => x.JiraTestMasterDto.GroupKey),
                    IsIndividual = isIndividual
                };
                return output.TransformText();

            }
            catch (Exception e)
            {
                logger.LogError(e.Message + e.StackTrace);
                throw;
            }
           
           
          

        }
        public async Task<string> GetJiraScreenTestTemplate(IList<JiraTestResult> lstTestResult)
        {
            string head = HeadHtml();
            var output = new JiraScreenTestOutput();

            output.Model = new JiraScreenTestOutPutModel()
            {
                Header = head,
                TestResult = lstTestResult.OrderBy(x => x.JiraTestMasterDto.StepId).
                    ToLookup(x => $"{x.JiraTestMasterDto.GroupKey}_{x.JiraTestMasterDto.Scenario}",x=>x.ScreenTestResult)
               
            };

            return output.TransformText();


        }


        public string GetJiraBusinessExceptionTemplate(string groupKey)
        {
            string head = HeadHtml();
            var businessExceptions = businessExceptionFactory.GetBusinessExceptionList().Where(x=> x.JiraMaster.GroupKey==groupKey);
            var output = new JiraBusinessExceptionOutput();
            output.Model = new JiraBusinessExceptionOutPutModel()
            {
                Header = head,
                TestResult = businessExceptions.ToLookup(x => x.JiraMaster.Scenario)

            };

            return output.TransformText();


        }

       

        private string GetHeaderSection1(JiraMetaDataDto metaData)
        {
            return 
                   "<div class=\"col-sm rounded\">" +
                   "<table class=\"table\">" +
                   "<caption>System Summary</caption><thead>" +
                   $"<tr><th>Jira Version</th> <th>{metaData.JiraVersion}</th>  </tr> " +
                   $"<tr><th>Jira Url</th> <th>{metaData.JiraUrl}</th>  </tr>" +
                   $"<tr><th>JiraAccount</th> <th>{metaData.JiraAccount}</th>  </tr>" +
                   $"<tr><th>TestRunBy</th> <th>{metaData.TestRunBy}</th>  </tr>" +
                   $"</thead><tbody></tbody></table>" +
                   $"</div>";


        }
        private string GetHeaderSection2(IList<JiraTestResult> lstTestResult)
        {            
            var totalTest = lstTestResult.Select(x=>x.JiraTestMasterDto.GroupKey).Distinct();
            //var passedTest = lstTestResult.Where(x => x.TestPassed).Count();
            var failedTest = lstTestResult.Where(x => !x.TestPassed).Select(x => x.JiraTestMasterDto.GroupKey).Distinct();

            var passedtest = totalTest.Except(failedTest).Count();
            var passedTestPercentage = Math.Round(((decimal)passedtest / totalTest.Count()) * 100, 2);
            var failedTestPercentage = Math.Round(((decimal)failedTest.Count() / totalTest.Count()) * 100, 2);

            return 
                   "<div class=\"col-sm rounded\">" +
                   "<table class=\"table\">" +
                   "<caption>WorkFlow Execution Details</caption><thead>" +
                    $"<tr><th></th> <th></th>  <th>Percentage(%)</th></tr> " +
                   $"<tr><th>Total Tests/WorkFlow Executed</th> <th>{totalTest.Count()}</th>  <th>100</th></tr> " +
                 $"<tr class=\"table-success\"><th>Passed Tests</th> <th>{passedtest}</th><th> {passedTestPercentage} </th> </tr> " +
                 //$"<tr class=\"table-success\"><th>Passed Tests(%)</th> <th>{(passedtest/ totalTest.Count())*100}</th></tr> " +
                   $"<tr class=\"table-danger\"><th>Failed Tests</th> <th>{failedTest.Count()}</th><th> {failedTestPercentage} </th></tr> " +
                   //$"<tr class=\"table-danger\"><th>Failed Tests(%)</th> <th>{(failedTest.Count()/ totalTest.Count())*100}</th></tr> " +

                   $"</thead><tbody></tbody></table>" +
                   $"</div>";


        }

        private string GetHeaderSection3(string assemblyVersion,DateTime startTime)
        {
            var diff = DateTime.Now.Subtract(startTime);
            return $"<div class=\"col-sm rounded\"><table class=\"table\">" +
                   $"<caption>Test Automation Details</caption><thead>" +
                   $"<tr><th>JiraTestProVersion</th><th>{assemblyVersion}</th></tr>" +
                   $"<tr><th>TestStartedAt:</th><th>{startTime.GetDisplayFormatDateTime()}</th></tr>" +
                   $"<tr><th>TestCompletedAt:</th><th>{DateTime.Now.GetDisplayFormatDateTime()}</th></tr>" +
                   $"<tr><th>Elapsed Time:</th><th>{string.Format("{0:D2}",diff.Hours)}:{string.Format("{0:D2}",diff.Minutes)} (HH:MM)</th></tr>" +
                   $"</thead><tbody><tr><td></td><td></td></tr></tbody></table></div>";

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
