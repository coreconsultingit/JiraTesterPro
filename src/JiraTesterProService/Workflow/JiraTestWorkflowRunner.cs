using JiraTesterProService.ExcelHandler;
using JiraTesterProService.FileHandler;
using JiraTesterProService.JiraParser;
using Serilog;

namespace JiraTesterProService.Workflow;

public class JiraTestWorkflowRunner : IJiraTestWorkflowRunner
{
    private IJiraTestScenarioReader jiraTestScenarioReader;
    private IFileFactory fileFactory;
    private IDataTableParser<JiraTestMasterDto> jiraTestMasterParser;
    private IJiraTestStartegyFactory jiraTestStartegyFactory;
    private IJiraTestResultWriter jiraTestResultWriter;
    private IJiraFileConfigProvider jiraFileConfigProvider;
    public JiraTestWorkflowRunner(IJiraTestScenarioReader jiraTestScenarioReader, IFileFactory fileFactory, IDataTableParser<JiraTestMasterDto> jiraTestMasterParser, IJiraTestStartegyFactory jiraTestStartegyFactory, IJiraTestResultWriter jiraTestResultWriter, IJiraFileConfigProvider jiraFileConfigProvider)
    {
        this.jiraTestScenarioReader = jiraTestScenarioReader;
        this.fileFactory = fileFactory;
        this.jiraTestMasterParser = jiraTestMasterParser;
        this.jiraTestStartegyFactory = jiraTestStartegyFactory;
        this.jiraTestResultWriter = jiraTestResultWriter;
        this.jiraFileConfigProvider = jiraFileConfigProvider;
    }

    public async Task<IList<JiraTestResult>> RunJiraWorkflow()
    {
        var dto = jiraFileConfigProvider.GetFileConfigDto();
        IList<JiraTestMasterDto> lstJiraTestItems = new List<JiraTestMasterDto>();
        if (dto.MasterTestFile == null)
        {
            var testFileData =
                await fileFactory.GetDataTableFromFile(new FileInfo(dto.InputJiraTestFile));

            var parsedItems = jiraTestMasterParser.ConvertDataTableToList(testFileData, null);
            if (parsedItems.lstValidationMessage.Any())
            {
                foreach (var message in parsedItems.lstValidationMessage)
                {
                    Log.Logger.Error(message);
                }
                throw new Exception("Error parsing the file");
            }

            lstJiraTestItems = parsedItems.lstItems.ToList();
        }
        else
        {
           
            lstJiraTestItems = await jiraTestScenarioReader.GetJiraMasterDtoFromMatrix(dto.MasterTestFile);
        }

        var testResult = await jiraTestStartegyFactory.GetJiraTestStrategyResult(lstJiraTestItems);

        await jiraTestResultWriter.WriteTestResult(testResult);

        return testResult;
    }
}