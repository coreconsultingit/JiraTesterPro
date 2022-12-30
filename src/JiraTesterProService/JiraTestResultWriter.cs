using System.Text.Encodings.Web;
using JiraTesterProService.JiraParser;
using JiraTesterProService.OutputTemplate;
using Microsoft.AspNetCore.Html;

namespace JiraTesterProService;

public class JiraTestResultWriter : IJiraTestResultWriter
{
    private IJiraTestOutputGenerator jiraTestOutputGenerator;
    private IJiraFileConfigProvider fileConfigProvider;
    private ILogger<JiraTestResultWriter> logger;
    public JiraTestResultWriter(IJiraTestOutputGenerator jiraTestOutputGenerator, IJiraFileConfigProvider fileConfigProvider, ILogger<JiraTestResultWriter> logger)
    {
        this.jiraTestOutputGenerator = jiraTestOutputGenerator;
        this.fileConfigProvider = fileConfigProvider;
        this.logger = logger;
    }

    public async Task<IList<JiraTestResultWriterResult>> WriteTestResult(DateTime startTime,IList<JiraTestResult> lstJiraTestResult)
    {
        var lstFileResult = new List<JiraTestResultWriterResult>();
        var hasexception = false;
        try
        {
            if (File.Exists(fileConfigProvider.OutputJiraTestFilePathWithMasterFile))
            {
                File.Delete(fileConfigProvider.OutputJiraTestFilePathWithMasterFile);
            }
            //TODO: handle all this in central place
            var directory = new FileInfo(fileConfigProvider.OutputJiraTestFilePathWithMasterFile).DirectoryName;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            var output = jiraTestOutputGenerator.GetJiraOutPutTemplate(lstJiraTestResult, startTime);
            await File.WriteAllTextAsync(fileConfigProvider.OutputJiraTestFilePathWithMasterFile, await output);
            var masterfolder = $"{Path.Combine(fileConfigProvider.OutputJiraTestFilePathWithMaster, DateTime.Now.ToString("yyyyMMdd"))}";

            var zipfileName = $"{masterfolder}.zip";
            if (File.Exists(zipfileName))
            {
                File.Delete(zipfileName);
            }
            logger.LogInformation($"Creating master zip file {zipfileName} from {masterfolder}");
            ZipFile.CreateFromDirectory(masterfolder, zipfileName);

            lstFileResult.Add(new JiraTestResultWriterResult("All Scenarios", zipfileName, lstJiraTestResult.Any(x=>x.TestStatus==JiraTestStatusConst.Failed)? "background-color:#f1b0b7" : "background-color:#8fd19e"
                , fileConfigProvider.OutputJiraTestFilePathWithMasterFile, zipfileName));

            var groupedData = lstJiraTestResult.ToLookup(x => x.JiraTestMasterDto.GroupKey);
            
            foreach (var groupkey in groupedData)
            {
               
                var jiraTestResults = groupedData[groupkey.Key].ToList();
                var businessException = jiraTestOutputGenerator.GetJiraBusinessExceptionTemplate(groupkey.Key);
                var screenresult = await jiraTestOutputGenerator.GetJiraScreenTestTemplate(jiraTestResults );
                var outputgroupkey = jiraTestOutputGenerator.GetJiraOutPutTemplate(jiraTestResults, startTime,true);
                var outputpath = fileConfigProvider.OutputJiraTestFilePathWithMasterFileByGroupKey(groupkey.Key);

                FileInfo fs = new FileInfo(outputpath.outputfile);
                if (!Directory.Exists(fs.DirectoryName))
                {
                    Directory.CreateDirectory(fs.DirectoryName);
                }


                try
                {
                    try
                    {
                        await File.WriteAllTextAsync($"{outputpath.outputfile.Replace("TestOutPut.html", "BusinessException.html")}", businessException);
                    }
                    catch (Exception e)
                    {
                        hasexception = true;
                      logger.LogError($"Error writing business exception for {groupkey} {e.Message}");
                    }


                    try
                    {
                        await File.WriteAllTextAsync($"{outputpath.outputfile.Replace("TestOutPut.html", "ScreenTestResult.html")}", screenresult);

                    }
                    catch (Exception e)
                    {
                        hasexception = true;
                        logger.LogError($"Error writing screentest result exception for {groupkey} {e.Message}");
                    }
                  
                    logger.LogInformation($"Writing the output file at {outputpath.outputfile}");

                    try
                    {
                        await File.WriteAllTextAsync($"{outputpath.outputfile}", await outputgroupkey);

                    }
                    catch (Exception e)
                    {
                        hasexception = true;
                        logger.LogError($"Error writing output result exception for {groupkey} {e.Message}");
                    }
                    var groupzipfileName = $"{outputpath.zipfilepath}\\{groupkey.Key}.zip";


                    logger.LogInformation($"Writing the zip file at {groupzipfileName} from {outputpath.zipfilepath}\\{groupkey.Key}");
                    if (File.Exists(groupzipfileName))
                    {
                        File.Delete(groupzipfileName);
                    }

                    ZipFile.CreateFromDirectory($"{outputpath.zipfilepath}\\{groupkey.Key}", groupzipfileName);


                    lstFileResult.Add(new JiraTestResultWriterResult(groupkey.Key, groupzipfileName, jiraTestResults.Any(x => x.TestStatus == JiraTestStatusConst.Failed) ? "background-color:#f1b0b7" : "background-color:#8fd19e",  fileConfigProvider.OutputJiraTestFilePathWithMasterFile, zipfileName));


                }
                catch (Exception e)
                {
                    hasexception = true;
                    logger.LogError(e.Message);
                }
            }

            if (hasexception)
            {
                throw new Exception("Error generating the files. Please check the log for more details");
            }
            return lstFileResult;
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            logger.LogError(e.StackTrace);
            throw;
        }

    }
}