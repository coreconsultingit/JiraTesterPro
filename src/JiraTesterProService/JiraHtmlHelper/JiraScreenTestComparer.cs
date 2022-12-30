using JiraTesterProData.JiraMapper;
using JiraTesterProService.BusinessExceptionHandler;
using System.Reflection.Metadata;

namespace JiraTesterProService.JiraHtmlHelper;

public class JiraScreenTestComparer : IJiraScreenTestComparer
{
    private IBusinessExceptionFactory businessExceptionFactory;
    private ILogger<JiraScreenTestComparer> logger;
    public JiraScreenTestComparer(IBusinessExceptionFactory businessExceptionFactory, ILogger<JiraScreenTestComparer> logger)
    {
        this.businessExceptionFactory = businessExceptionFactory;
        this.logger = logger;
    }

    public IList<ScreenTestResult> CompareScreenFields(IList<JiraHtmlFieldDto> lstJiraHtmlFields, JiraTestMasterDto dto)
    {
        var lstScreenTestResult = new List<ScreenTestResult>();
        var jiraHtmlFields = lstJiraHtmlFields.ToLookup(x => x.Name,StringComparer.OrdinalIgnoreCase);
            
        foreach (var field in dto.ScreenTestDto)
        {
            var jiraHtml = jiraHtmlFields[field.FieldName].FirstOrDefault();
            var screenTest = new ScreenTestResult()
            {
                ScreenTestDto = field, TestPassed = true, HtmlFieldDto = jiraHtml
            };
            if (jiraHtml== null)
            {
                var message = $"No field with name {field.FieldName} found in the Jira screen";
                businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Critical, message,
                    dto, TestType.Screen));
                screenTest.TestPassed = false;
                screenTest.Comment = message;
                continue;
            }
            if (jiraHtml.IsRequired != field.IsMandatory)
            {

                var message =
                    $"{field.FieldName} required flag do not match.Screen has {jiraHtml.IsRequired}. Test case has {field.IsMandatory}";

                screenTest.TestPassed = false;
                screenTest.Comment = message;
                businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Critical, message,
                    dto, TestType.Screen));

            }

            //if (jiraHtml.InValidValuelist.Any())
            //{
            //    var message =
            //        $"{field.FieldName} has invalid value {string.Join(",",jiraHtml.InValidValuelist)}. ";

            //    screenTest.TestPassed = false;
            //    screenTest.Comment = message;
            //    businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Critical, message,
            //        dto, TestType.Screen));
            //}
            //TODO: Option need to be sorted
            if (jiraHtml.ElementType != field.SystemField)
            {
                if (field.SystemField.EqualsWithIgnoreCase("option"))
                {
                    if (!ElementType.IsMultiValue(jiraHtml.ElementType) &&
                        jiraHtml.ElementType != ElementType.SingleDropdown)
                    {
                        var message =
                            $"{field.FieldName} field definition do not match.Screen has {jiraHtml.ElementType}. Test case has {field.SystemField}";
                        screenTest.TestPassed = false;
                        screenTest.Comment = message;
                        businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Critical, message,
                            dto, TestType.Screen));
                    }
                }
                else
                {
                    var message =
                        $"{field.FieldName} field definition do not match.Screen has {jiraHtml.ElementType}. Test case has {field.SystemField}";

                    screenTest.TestPassed = false;
                    screenTest.Comment = message;
                    businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Critical, message,
                        dto, TestType.Screen));
                }
                
              
            }

            if (jiraHtml.ElementType == ElementType.Checkbox ||
                jiraHtml.ElementType == ElementType.SingleDropdown ||
                jiraHtml.ElementType == ElementType.MultiDropdown)
            {

                if (!string.IsNullOrEmpty(field.ListValuesAvailable))
                {
                    var testvalues = field.ListValuesAvailable.Split(new string[] { "\r\n", "\n" },
                        StringSplitOptions.None);
                    var listCompared = jiraHtml.AvailableValues.Select(x => x.Trim()).ToList()
                        .IsListEqual(testvalues.Select(x=>x.Trim()).Where(x=>!string.IsNullOrEmpty(x)).ToList());
                        
                    if (!listCompared.isEqual)
                    {
                        string sourceOnlyMessage = String.Empty;
                        string destinationOnlyMessage = String.Empty;
                        if (listCompared.sourceOnly.Any())
                        {
                            sourceOnlyMessage =
                                $"Screen has additional items{string.Join(Environment.NewLine, listCompared.sourceOnly)}.";
                        }
                        if (listCompared.destinationOnly.Any())
                        {
                            destinationOnlyMessage =
                                $" Test case has {string.Join(Environment.NewLine, listCompared.destinationOnly)}";
                        }
                        var message =
                            $"{field.FieldName} available options do not match." +
                            $"{sourceOnlyMessage}." +
                            $" {destinationOnlyMessage}";

                        screenTest.TestPassed = false;
                        screenTest.Comment = message;
                        businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Critical, message,
                            dto, TestType.Screen));

                     
                    }
                }
                   
            }

            

            lstScreenTestResult.Add(screenTest);

        }
        if (!dto.ScreenTestDto.Any())
        {
            foreach(var field in lstJiraHtmlFields)
            {
                var screenTest = new ScreenTestResult()
                {
                    ScreenTestDto = new ScreenTestDto(),
                    TestPassed = true,
                    HtmlFieldDto = field
                };
                lstScreenTestResult.Add(screenTest);

            }

           

        }

        return lstScreenTestResult;
    }
}