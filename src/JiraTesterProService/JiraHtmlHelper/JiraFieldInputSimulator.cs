using JiraTesterProData.JiraMapper;
using JiraTesterProService.BusinessExceptionHandler;
using PuppeteerSharp;
using PuppeteerSharp.Input;

namespace JiraTesterProService.JiraHtmlHelper;

public class JiraFieldInputSimulator : IJiraFieldInputSimulator
{
    private IJiraFieldInputProvider jiarFieldInputProvider;
    private int fieldInputInterval;
    private int textfieldInputInterval;
    private int waitTimeOutInterval;
    private ILogger<JiraFieldInputSimulator> jiraFieldInputLogger;
    private IBusinessExceptionFactory businessExceptionFactory;

    public JiraFieldInputSimulator(IJiraFieldInputProvider jiarFieldInputProvider, IConfiguration configuration, IBusinessExceptionFactory businessExceptionFactory, ILogger<JiraFieldInputSimulator> jiraFieldInputLogger)
    {
        this.jiarFieldInputProvider = jiarFieldInputProvider;
        
        this.businessExceptionFactory = businessExceptionFactory;
        this.jiraFieldInputLogger = jiraFieldInputLogger;
        fieldInputInterval = configuration.GetValue<int?>("FieldInputInterval") ?? 100;
        textfieldInputInterval = configuration.GetValue<int?>("TextFieldInputInterval") ?? 0;
        waitTimeOutInterval = configuration.GetValue<int?>("WaitTimeOutInterval") ?? 1000;
    }

    public async Task<JiraHtmlFieldDto> SimulateInput(IPage page, IElementHandle elementHandle, JiraHtmlFieldDto fielddto, JiraTestMasterDto dto)
    {
        try
        {
           
            if (fielddto.Name.EqualsWithIgnoreCase("attachment"))
            {
               // await page.Keyboard.PressAsync("Tab");
                return fielddto;
            }

            dto.SuppliedValues.TryGetValue(fielddto.Name, out string suppliedvalval);

            if (string.IsNullOrEmpty(suppliedvalval) &&  dto.Action == JiraActionEnum.Update.ToString())
            {
                if (!dto.SuppliedValues.TryGetValue(fielddto.Name, out string val))
                {
                    var handler = await elementHandle.QuerySelectorAsync($"#{fielddto.FieldId}");
                    if (handler != null)
                    {
                        var existingvalue = await handler.EvaluateFunctionAsync<string>("node=>node.value");
                        if (!string.IsNullOrEmpty(existingvalue))
                        {
                            return fielddto;
                        }
                    }
                }
            }
            var inputval = jiarFieldInputProvider.GetParsedFieldValue(fielddto, dto);
            if (!string.IsNullOrEmpty(inputval))
            {
                if (inputval.GetStringSplitOnNewLine().IsListEqual(fielddto.SelectedValue).isEqual)
                {
                    return fielddto;
                }

                if (fielddto.ElementType.EqualsWithIgnoreCase(ElementType.TextBox) || fielddto.ElementType.EqualsWithIgnoreCase(ElementType.DatePicker) || fielddto.ElementType.EqualsWithIgnoreCase(ElementType.TextArea))
                {
                    await TypeFieldValue(page, $"#{fielddto.FieldId}", inputval, elementHandle);
                    fielddto.SelectedValue.Add(inputval);
                }

                else if (fielddto.ElementType.EqualsWithIgnoreCase(ElementType.Checkbox) || ElementType.IsMultiValue(fielddto.ElementType) || fielddto.ElementType.EqualsWithIgnoreCase(ElementType.SingleDropdown))
                {
                    var arValues = inputval.GetStringSplitOnNewLine();
                    int iCounter = 1;
                    foreach (var val in fielddto.AvailableValues)
                    {
                        if (arValues.Contains(val))
                        {
                            if (fielddto.ElementType.EqualsWithIgnoreCase(ElementType.Checkbox))
                            {
                                var controlId = $"{fielddto.FieldId}-{iCounter}";
                                await ClickcheckboxValue(page, controlId, elementHandle);
                                fielddto.SelectedValue.Add(val);
                            }

                            else
                            {
                                await SetDropdownValue(page, fielddto.FieldId, val, elementHandle);
                                fielddto.SelectedValue.Add(val);
                                if (fielddto.ElementType.EqualsWithIgnoreCase(ElementType.SingleDropdown))
                                {
                                    break;
                                }
                            }

                        }
                        iCounter = iCounter + 1;
                    }

                    var invalidValueList = arValues.Except(fielddto.AvailableValues);
                    if (invalidValueList.Any())
                    {
                        fielddto.InValidValuelist.AddRange(invalidValueList);
                    }

                }
                else
                {
                    businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Critical, $"This control type hasn't been handled yet {fielddto.ElementType}", dto ,TestType.FieldValue));
                }
            }

            return fielddto;
        }
        catch (Exception e)
        {
            jiraFieldInputLogger.LogError(e.Message);
            businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Critical, $"Error getting value for  {fielddto.ElementType} {e.Message}", dto, TestType.FieldValue));
        }

        return fielddto;


    }

    private async Task TypeFieldValue(IPage page, string fieldSelector, string value, IElementHandle elementHandle)
    {
       
        var waitSelectorOptions = new WaitForSelectorOptions()
        {
            Timeout = waitTimeOutInterval
        };
        var handler = await elementHandle.WaitForSelectorAsync($"{fieldSelector}", waitSelectorOptions);

        await handler.FocusAsync();
        var option = new TypeOptions();
        if (textfieldInputInterval != 0)
        {
            option.Delay = textfieldInputInterval;

        }
        await page.EvaluateFunctionAsync($"()=>document.querySelector('{fieldSelector}').value = \"\"", handler);
        await page.TypeAsync($"{fieldSelector}", value, option);
        await page.Keyboard.PressAsync("Tab");
    }

    private  async Task ClickcheckboxValue(IPage page, string fieldSelector, IElementHandle elementHandle)
    {
        var waitSelectorOptions = new WaitForSelectorOptions()
        {
            Timeout = waitTimeOutInterval
        };
        var handler =await elementHandle.WaitForSelectorAsync($"#{fieldSelector}", waitSelectorOptions);

        await handler.FocusAsync();
        await handler.ClickAsync(new ClickOptions() { Delay = fieldInputInterval });
        await page.Keyboard.PressAsync("Tab");
    }

    private  async Task SetDropdownValue(IPage page, string dropdownId, string value, IElementHandle elementHandle)
    {
        var elementHandles = await elementHandle.XPathAsync($"//*[@id = \"{dropdownId}\"]/option[normalize-space(text()) = \"{value}\"]");

        if (elementHandles.Length > 0)
        {
            var chosenOption = elementHandles[0];
            var jsHandle = await chosenOption.GetPropertyAsync("value");
            var choseOptionValue = await jsHandle.JsonValueAsync<string>();
           //ToDO: sort with the handler
            await page.FocusAsync($"#{dropdownId}");
            await page.SelectAsync($"#{dropdownId}", choseOptionValue);
        }
        else
        {
            await page.FocusAsync($"#{dropdownId}");
            await page.SelectAsync($"#{dropdownId}", value);
        } await page.Keyboard.PressAsync("Tab");
    }
}