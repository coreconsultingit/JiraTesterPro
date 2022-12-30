

namespace JiraTesterProTestFixture;

[TestFixture]
public class JiraFieldInputProviderTestFixture:JiraTestFixtureBase
{
    [Test]
    public void Is_Able_To_Assign_Date_IfMissing()
    {
        var provider = _serviceProvider.GetService<IJiraFieldInputProvider>();
        var value = provider.GetParsedFieldValue(new JiraHtmlFieldDto()
        {
            ElementType = ElementType.DatePicker, Name = "CalTest",IsRequired = true, IsVisible = true
        }, new JiraTestMasterDto()
        {
            Scenario = "TestScenario"
        });

        Assert.AreEqual(DateTime.Now.GetJiraFormatDateTime(),value);
    }
}