using JiraTesterProService;
using JiraTesterProService.ExcelHandler;
using Microsoft.Extensions.DependencyInjection;

namespace JiraTesterProTestFixture;

public class JiraTestScenarioReaderTestFixture: JiraTestFixtureBase
{
    private IJiraTestScenarioReader jiraTestScenarioReader;
    private IJiraTestStartegyFactory testStartegyFactory;
    [SetUp]
    public void SetUp()
    {
        jiraTestScenarioReader = _serviceProvider.GetService<IJiraTestScenarioReader>();
        testStartegyFactory = _serviceProvider.GetService<IJiraTestStartegyFactory>();
    }

    [Test]
    public async Task Is_Able_ToReadExcelRules()
    {
        var jiraMasterDto =
            jiraTestScenarioReader.GetJiraMasterDtoFromMatrix(@"..\..\..\..\..\docs\Jira BUG Matrix.xlsx");
        Assert.AreEqual(11, jiraMasterDto.Count);

        var testResult = await testStartegyFactory.GetJiraTestStrategyResult(jiraMasterDto);




    }
}