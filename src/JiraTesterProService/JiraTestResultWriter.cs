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

    public async Task<bool> WriteTestResult(IList<JiraTestResult> lstJiraTestResult)
    {
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

            var output = jiraTestOutputGenerator.GetJiraOutPutTemplate(lstJiraTestResult);

            await File.WriteAllTextAsync(fileConfigProvider.OutputJiraTestFilePathWithMasterFile, await output);

            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            throw;
        }

    }
}