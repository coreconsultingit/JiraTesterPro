using System.Text.Encodings.Web;
using JiraTesterProService.OutputTemplate;
using Microsoft.AspNetCore.Html;

namespace JiraTesterProService;

public class JiraTestResultWriter : IJiraTestResultWriter
{
    private IJiraTestOutputGenerator JiraTestOutputGenerator;
    public JiraTestResultWriter(IJiraTestOutputGenerator jiraTestOutputGenerator)
    {
        JiraTestOutputGenerator = jiraTestOutputGenerator;
    }

    public async Task<bool> WriteTestResult(IList<JiraTestResult> lstJiraTestResult, string filepath)
    {
        try
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            var output = JiraTestOutputGenerator.GetJiraOutPutTemplate(lstJiraTestResult);

            await File.WriteAllTextAsync(filepath, output);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }
}