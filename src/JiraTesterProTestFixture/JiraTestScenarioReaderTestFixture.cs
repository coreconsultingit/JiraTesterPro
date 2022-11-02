using JiraTesterProService;
using JiraTesterProService.ExcelHandler;
using Microsoft.Extensions.DependencyInjection;
using System;

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
    public async Task Is_Able_ToReadExcelRules_And_GenerateFile()
    {
        var jiraMasterDto =
            await jiraTestScenarioReader.GetJiraMasterDtoFromMatrix(@"..\..\..\..\..\docs\Jira BUG Matrix.xlsx");
        Assert.That(jiraMasterDto.Count, Is.EqualTo(11));

        var testResult = await testStartegyFactory.GetJiraTestStrategyResult(jiraMasterDto);


        var writer = _serviceProvider.GetService<IJiraTestResultWriter>();
        writer.WriteTestResult(testResult,"JiraTest.html");

    }
}