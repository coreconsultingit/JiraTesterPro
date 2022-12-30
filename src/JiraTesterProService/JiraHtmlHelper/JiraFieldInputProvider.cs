using JiraTesterProData.JiraMapper;
using JiraTesterProService.BusinessExceptionHandler;

namespace JiraTesterProService.JiraHtmlHelper;

public class JiraFieldInputProvider : IJiraFieldInputProvider
{

    private IBusinessExceptionFactory businessExceptionFactory;
    private ILogger<JiraScreenTestComparer> logger;

    public JiraFieldInputProvider(IBusinessExceptionFactory businessExceptionFactory, ILogger<JiraScreenTestComparer> logger)
    {
        this.businessExceptionFactory = businessExceptionFactory;
        this.logger = logger;
    }

    public string GetParsedFieldValue(JiraHtmlFieldDto fielddto, JiraTestMasterDto dto)
    {
        if (!fielddto.IsVisible)
        {
            return String.Empty;
        } 
        string val;
        var fieldName = fielddto.Name;
        if (fieldName.Contains($"{Environment.NewLine}Required"))
        {
            businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Warning,$"Screen field {fieldName} Required field setup is incorrect",dto,TestType.Screen));
            fieldName = fieldName.Replace($"{Environment.NewLine}Required", "");
        }
        if (dto.SuppliedValues.TryGetValue(fieldName, out val))
        {
            var trimmedVal = val.Trim();
            GetValOrDefault(fielddto, trimmedVal, dto);
        }
        else
        {
            if (fielddto.Name.EqualsWithIgnoreCase("summary"))
            {
                return $"{dto.UniqueKey}" ;
            }
            if (!fielddto.IsRequired)
            {
                return val;
            }
            else
            {
                return GetValOrDefault(fielddto, val,dto);
            }
        }
        return val;

    }

    private string GetValOrDefault(JiraHtmlFieldDto fielddto,string val, JiraTestMasterDto dto)
    {
        if (fielddto.ElementType.EqualsWithIgnoreCase(ElementType.DatePicker))
        {
            if (string.IsNullOrEmpty(val))
            {
                val = DateTime.Now.ToString("d/MMM/yy");
                LogInformationBusinessException(fielddto, val, dto);
                return val;
            }
            else
            {
                if (DateTime.TryParse(val, out DateTime parsedDate))
                {
                    return parsedDate.ToString("d/MMM/yy");
                }
                else
                {
                    val = DateTime.Now.ToString("d/MMM/yy");
                    
                    businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Error,$"Input value {val} cannot be parsed as a valid date.Using default value", dto,TestType.FieldValue));
                    LogInformationBusinessException(fielddto, val, dto);
                }
            }
        }
        else if (fielddto.ElementType.EqualsWithIgnoreCase(ElementType.SingleDropdown) || ElementType.IsMultiValue(fielddto.ElementType))
        {
            if (string.IsNullOrEmpty(val))
            {
                val = fielddto.AvailableValues.Count() > 1 ? fielddto.AvailableValues[1] : fielddto.AvailableValues[0];
                LogInformationBusinessException(fielddto, val, dto);
                return val;

            }

            var values = val.GetStringSplitOnNewLine().IsListEqualWithDestinationCheck(fielddto.AvailableValues);
            if (!values.isEqual)
            {
                businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Error, $"Input value {val} has some unmatched options. {string.Join(Environment.NewLine,values.missing)}", dto, TestType.FieldValue));
            }

            return val;
        }
        else if (fielddto.ElementType.EqualsWithIgnoreCase(ElementType.TextBox) || fielddto.ElementType.EqualsWithIgnoreCase(ElementType.TextArea))
        {
            if (string.IsNullOrEmpty(val))
            {
                if (fielddto.Name.EqualsWithIgnoreCase("Assignee"))
                {
                    val = "Automatic";
                }
                else if (fielddto.Name.EqualsWithIgnoreCase("Component/s"))
                {
                    val = "ERT Internal";
                }
                else
                {
                    val = $"Test value for {fielddto.Name}";
                }
                    
                LogInformationBusinessException(fielddto, val, dto);



            }
        }
        return val;
    }

    private void LogInformationBusinessException(JiraHtmlFieldDto fielddto, string val, JiraTestMasterDto scenario)
    {
        businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Information, $"Input value {val} being used for {fielddto.Name}", scenario, TestType.FieldValue));
    }
}