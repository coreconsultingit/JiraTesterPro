using JiraTesterProData;
using JiraTesterProData.JiraMapper;
using JiraTesterProService.BusinessExceptionHandler;
using JiraTesterProService.JiraHtmlHelper;
using Microsoft.Extensions.DependencyInjection;

namespace JiraTesterProTestFixture;

[TestFixture]
public class JiraScreenTestComparerTestFixture:JiraTestFixtureBase
{

    [Test]
    public void No_Business_Exception_When_AvailableValuesMatches()
    {
        var screenTestComparer = _serviceProvider.GetService<IJiraScreenTestComparer>();

        var jiraHtmlField = new JiraHtmlFieldDto()
        { 
            Name="Component/s",
            ElementType = "select-one", AvailableValues = new List<string>()
            {
                "One", "Three", "Two"
            }
        };
        var jiraDto = new JiraTestMasterDto()
        {
            Scenario = "Test1",GroupKey = "k1",
            ScreenTestDto = new List<ScreenTestDto>()
            {
                new ScreenTestDto()
                {
                    FieldName = "Component/s", SystemField = "select-one", ListValuesAvailable = "One \n Two\n Three"
                }
            }
        };
    
        screenTestComparer.CompareScreenFields(new List<JiraHtmlFieldDto>(){ jiraHtmlField },jiraDto);

        var businessException = _serviceProvider.GetService<IBusinessExceptionFactory>();

        Assert.AreEqual(0, businessException.GetBusinessExceptionList().Where(x => x.JiraMaster.GroupKey == "k1").Count());


    }

    [Test]
    public void Business_Exception_When_AvailableValuesDoNotMatches()
    {
        var screenTestComparer = _serviceProvider.GetService<IJiraScreenTestComparer>();

        var jiraHtmlField = new JiraHtmlFieldDto()
        {
            Name = "Component/s",
            ElementType = "select-one",
            AvailableValues = new List<string>()
            {
                "One", "Three"
            }
        };
        var jiraDto = new JiraTestMasterDto()
        {
            Scenario = "Test2",GroupKey = "k0",
            ScreenTestDto = new List<ScreenTestDto>()
            {
                new ScreenTestDto()
                {
                    FieldName = "Component/s", SystemField = "select-one", ListValuesAvailable = "One \n Two\n Three"
                }
            }
        };

        screenTestComparer.CompareScreenFields(new List<JiraHtmlFieldDto>() { jiraHtmlField }, jiraDto);

        var businessException = _serviceProvider.GetService<IBusinessExceptionFactory>();

        Assert.AreEqual(1, businessException.GetBusinessExceptionList().Where(x => x.JiraMaster.GroupKey == "k0").Count());

    }

}