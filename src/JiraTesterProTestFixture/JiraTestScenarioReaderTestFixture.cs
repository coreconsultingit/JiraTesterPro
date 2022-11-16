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
        var filePath = @"..\..\..\..\..\docs\Jira BUG Matrix.xlsx";
        var jiraMasterDto =
            await jiraTestScenarioReader.GetJiraMasterDtoFromMatrix(filePath);
        Assert.That(jiraMasterDto.Count, Is.EqualTo(11));

        var jiraFileConfigProvider = _serviceProvider.GetService<IJiraFileConfigProvider>();

        jiraFileConfigProvider.InitializeConfig(new FileConfigDto()
            {
                MasterTestFile = filePath,
                OutputJiraTestFilePath = new FileInfo(Assembly.GetExecutingAssembly().FullName??@"..\").DirectoryName
            });

        var testResult = await testStartegyFactory.GetJiraTestStrategyResult(jiraMasterDto);


        var writer = _serviceProvider.GetService<IJiraTestResultWriter>();

        var result = await writer.WriteTestResult(testResult);

        Assert.IsTrue(result);
        
        

    }
}