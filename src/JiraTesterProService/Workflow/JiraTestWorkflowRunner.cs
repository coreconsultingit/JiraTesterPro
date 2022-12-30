using JiraTesterProService.BusinessExceptionHandler;
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
    private IJiraRefProvider jiraRefProvider;
    private IBusinessExceptionFactory businessExceptionFactory;
    public JiraTestWorkflowRunner(IJiraTestScenarioReader jiraTestScenarioReader, IFileFactory fileFactory, IDataTableParser<JiraTestMasterDto> jiraTestMasterParser, IJiraTestStartegyFactory jiraTestStartegyFactory, IJiraTestResultWriter jiraTestResultWriter, IJiraFileConfigProvider jiraFileConfigProvider, IJiraRefProvider jiraRefProvider, IBusinessExceptionFactory buisExceptionFactory)
    {
        this.jiraTestScenarioReader = jiraTestScenarioReader;
        this.fileFactory = fileFactory;
        this.jiraTestMasterParser = jiraTestMasterParser;
        this.jiraTestStartegyFactory = jiraTestStartegyFactory;
        this.jiraTestResultWriter = jiraTestResultWriter;
        this.jiraFileConfigProvider = jiraFileConfigProvider;
        this.jiraRefProvider = jiraRefProvider;
        this.businessExceptionFactory = buisExceptionFactory;
    }

    public async Task<JiraWorkFlowResult> RunJiraWorkflow()
    {
        var dto = jiraFileConfigProvider.GetFileConfigDto();
        IList<JiraTestMasterDto> lstJiraTestItems = new List<JiraTestMasterDto>();

        jiraRefProvider.Clear();
        businessExceptionFactory.ClearException();
        DateTime startTime = DateTime.Now;
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

        var fileResult = await jiraTestResultWriter.WriteTestResult(startTime,testResult);

        return new JiraWorkFlowResult()
        {
            IsSuccessfull = true,
            JiraTestResultWriterResult = fileResult,
            Results = testResult
        };
    }


}