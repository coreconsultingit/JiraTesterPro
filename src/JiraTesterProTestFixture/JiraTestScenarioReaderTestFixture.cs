using JiraTesterProService;
using JiraTesterProService.ExcelHandler;
using Microsoft.Extensions.DependencyInjection;

namespace JiraTesterProTestFixture;

public class JiraTestScenarioReaderTestFixture: JiraTestFixtureBase
{
    private IJiraTestScenarioReader jiraTestScenarioReader;

    [SetUp]
    public void SetUp()
    {
        jiraTestScenarioReader = _serviceProvider.GetService<IJiraTestScenarioReader>();
    }

    [Test]
    public void Is_Able_ToReadExcelRules()
    {
        var jiraMasterDto =
            jiraTestScenarioReader.GetJiraMasterDtoFromMatrix(@"..\..\..\..\..\docs\Jira BUG Matrix.xlsx");
        Assert.AreEqual(11, jiraMasterDto.Count);
    }
}