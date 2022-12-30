using JiraTesterProData;
using JiraTesterProService;
using JiraTesterProService.JiraParser;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

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
            await jiraTestScenarioReader.GetJiraMasterDtoFromMatrix(testFilePath);
        Assert.That(jiraMasterDto.Count, Is.EqualTo(102));

      

        var testResult = await testStartegyFactory.GetJiraTestStrategyResult(jiraMasterDto);


        var writer = _serviceProvider.GetService<IJiraTestResultWriter>();

        var result = await writer.WriteTestResult(DateTime.Now, testResult);

        Assert.IsNotEmpty(result);
        
        

    }
}