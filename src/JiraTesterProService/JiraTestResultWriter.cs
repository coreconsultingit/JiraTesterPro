using System.Text.Encodings.Web;
using JiraTesterProService.JiraParser;
using JiraTesterProService.OutputTemplate;
using Microsoft.AspNetCore.Html;

namespace JiraTesterProService;

public class JiraTestResultWriter : IJiraTestResultWriter
{
    private IJiraTestOutputGenerator JiraTestOutputGenerator;
    private JiraFileConfigProvider fileConfigProvider;
    public JiraTestResultWriter(IJiraTestOutputGenerator jiraTestOutputGenerator, JiraFileConfigProvider fileConfigProvider)
    {
        JiraTestOutputGenerator = jiraTestOutputGenerator;
        this.fileConfigProvider = fileConfigProvider;
    }

    public async Task<bool> WriteTestResult(IList<JiraTestResult> lstJiraTestResult)
    {
        try
        {
            if (File.Exists(fileConfigProvider.OutputJiraTestFilePathWithMasterFile))
            {
                File.Delete(fileConfigProvider.OutputJiraTestFilePathWithMasterFile);
            }

            var output = JiraTestOutputGenerator.GetJiraOutPutTemplate(lstJiraTestResult);

            await File.WriteAllTextAsync(fileConfigProvider.OutputJiraTestFilePathWithMasterFile, await output);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }
}