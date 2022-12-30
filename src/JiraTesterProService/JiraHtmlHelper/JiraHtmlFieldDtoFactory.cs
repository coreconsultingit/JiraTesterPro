using System.Diagnostics;
using JiraTesterProData.JiraMapper;
using JiraTesterProService.BusinessExceptionHandler;
using PuppeteerSharp;

namespace JiraTesterProService.JiraHtmlHelper
{
    public class JiraHtmlFieldDtoFactory: IJiraHtmlFieldDtoFactory
    {
        private IBusinessExceptionFactory businessExceptionFactory;
        private ILogger<JiraHtmlFieldDtoFactory> logger;

        public JiraHtmlFieldDtoFactory(IBusinessExceptionFactory businessExceptionFactory, ILogger<JiraHtmlFieldDtoFactory> logger)
        {
            this.businessExceptionFactory = businessExceptionFactory;
            this.logger = logger;
        }

        public async Task<JiraHtmlFieldDto> GetJiraHtmlFieldDto(JiraTestMasterDto dto, IPage page, IElementHandle elementHandle)
        {
            var fieldDto = new JiraHtmlFieldDto();

            try
            {
                fieldDto.IsRequired = await IsRequired(page, elementHandle);
                fieldDto.FieldGroupType = await GetFieldGroupOrFieldLabel(dto, page, elementHandle);
                fieldDto.IsVisible = await GetVisibility(page, elementHandle);
                //No need to check further for this control
                if (!fieldDto.IsVisible)
                {
                    return fieldDto;
                }
                var fieldIdAndName = await GetFieldNameAndId(dto, page, elementHandle, fieldDto.FieldGroupType, fieldDto.IsRequired);
                fieldDto.FieldId = fieldIdAndName.fieldId;
                fieldDto.Name = fieldIdAndName.fieldName;
                fieldDto.ElementType = fieldIdAndName.elementType;
                fieldDto.AvailableValues = fieldIdAndName.lstAvailableValues;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message + e.StackTrace);
                var message = $"Error handling for {await GetHandleString(elementHandle)} {e.Message}";
                businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Critical, message,
                    dto, TestType.Screen));
                

            }
            return fieldDto;
        }

        private async Task<bool> GetVisibility( IPage page, IElementHandle elementHandle)
        {
            if (elementHandle == null)
            {
                return false;
            }
            var style = await page.EvaluateFunctionAsync<string>("x=>x.getAttribute('style')", elementHandle);
            if ((style == "display: none;"))
            {
                return false;
            }

            return true;
        }

        private async Task<(string elementType, IList<string> lstAvailableValues)> GetElementType(JiraTestMasterDto dto, IPage page, IElementHandle elementHandle,string controltolook, FieldGroupOrFieldLabel fieldLabel, bool checkbox)
        {
            var waitSelectorOptions = new WaitForSelectorOptions()
            {
                Timeout = 1000
            };
            IElementHandle handle= elementHandle;
            var lstValues = new List<string>();

            try
            {
                if (controltolook.EqualsWithIgnoreCase("attachment"))
                {
                    //var attachment = await handle.QuerySelectorAllAsync(".issue-drop-zone__file.ignore-inline-attach");
                    var filecontrol = await page.WaitForSelectorAsync($".issue-drop-zone__file.ignore-inline-attach", waitSelectorOptions);

                    var filecontroltype =
                        await page.EvaluateFunctionAsync<string>("e => e.type", filecontrol);
                    return (filecontroltype, new List<string>());
                }
                if (checkbox || fieldLabel == FieldGroupOrFieldLabel.Group)
                {
                    if (checkbox)
                    {
                        var checkboxes = await handle.QuerySelectorAllAsync(".checkbox");
                        if (checkboxes != null)
                        {
                            foreach (var box in checkboxes)
                            {
                                var checkboxcontrol = await box.EvaluateFunctionAsync<string>(
                                    $"x=>x.getAttribute('{CssSelectorClass.CheckboxElementSelector}')");
                                if (checkboxcontrol != null)
                                {
                                    var txt = await box.EvaluateFunctionAsync<string>("el => el.innerText");
                                    lstValues.Add(txt);
                                }
                            }

                            return (ElementType.Checkbox, lstValues);

                        }
                        else
                        {
                            var message = $"No checkbox Handler found for {await GetHandleString(handle)}";
                            businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Critical, message,
                                dto, TestType.Screen));
                            

                        }


                    }
                    //handle = await page.WaitForXPathAsync($"//*[@name='{controltolook}']", waitSelectorOptions);

                }
                else
                {
                    if (controltolook.EqualsWithIgnoreCase("attachment"))
                    {
                        return (ElementType.FileUpload,lstValues);
                    }
                    
                }
            }
            catch (Exception e)
            {
                logger.LogWarning(e.Message + e.StackTrace);
                //if (fieldLabel == FieldGroupOrFieldLabel.Group)
                //{
                //   // handle = await page.WaitForSelectorAsync($"#{controltolook}", waitSelectorOptions);
                   
                //}
                //else
                //{
                //    handle = await page.WaitForXPathAsync($"//*[@name='{controltolook}']", waitSelectorOptions);
                //}

            }
            var control = await page.WaitForSelectorAsync($"#{controltolook}",waitSelectorOptions);
           
            
            var controltype =
                await page.EvaluateFunctionAsync<string>("e => e.type", control);
            var classused =
                await page.EvaluateFunctionAsync<string>("e => e.getAttribute('class')", control);

            if (controltype == "text" && classused.ContainsWithIgnoreCase("datepicker-input"))
            {
                return (ElementType.DatePicker, lstValues);
            }

            if (controltype !=null && controltype.StartsWith("select"))
            {
                var options = (await control.EvaluateFunctionAsync<string>("el => el.innerText")).GetStringSplitOnNewLine();
                lstValues.AddRange(options);
            }
            return (controltype,lstValues);
        }
        private async Task<FieldGroupOrFieldLabel> GetFieldGroupOrFieldLabel(JiraTestMasterDto dto, IPage page, IElementHandle elementHandle)
        {
            try
            {
                var field = await page.EvaluateFunctionAsync<string>("x=>x.getAttribute('class')", elementHandle);
                if (field.StartsWith("field-group"))
                {
                    return FieldGroupOrFieldLabel.FieldGroup;
                }
                else if (field.StartsWith("group"))
                {
                    return FieldGroupOrFieldLabel.Group;
                }
                else
                {
                    var message = $"No Label Handler found for {await GetHandleString(elementHandle)}";

                    businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Critical, message,
                        dto, TestType.Screen));


                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message + e.StackTrace);
                businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Critical, $"{await GetHandleString(elementHandle)}",
                    dto, TestType.Screen));
            }
            

            return FieldGroupOrFieldLabel.UnHandled;

        }


        private async Task<(string fieldId,string fieldName, string elementType, IList<string> lstAvailableValues)> GetFieldNameAndId(JiraTestMasterDto dto, IPage page, IElementHandle elementHandle, FieldGroupOrFieldLabel fieldLabel, bool isRequired)
        {
            string fieldId = String.Empty; 
            string fieldName=String.Empty;
            string elementType = String.Empty;
            var lstAvailableValues = new List<string>();
            bool checkBox = false;
            if (fieldLabel.Equals(FieldGroupOrFieldLabel.FieldGroup))
            {

                try
                {
                    var handle = await elementHandle.QuerySelectorAsync($"{CssSelectorClass.LabelSelector}");
                    if (handle != null)
                    {
                        fieldId = await page.EvaluateFunctionAsync<string>("x=>x.getAttribute('for')", handle);
                        //Temp hack. Find better way of handling this
                        if (fieldId.EqualsWithIgnoreCase("issuelinks"))
                        {
                            fieldName = "Linked Issues";
                            fieldId = "issuelinks-linktype";
                        }
                        else
                        {
                            var controlinput = await handle.EvaluateFunctionAsync<string>("el => el.innerText");
                            if (isRequired)
                            {
                                controlinput = controlinput.Replace("Required", "").ReplaceLastOccurrence("\n", "");
                            }
                            fieldName = controlinput;

                        }
                    }
                 
                   
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message + ex.StackTrace);
                }
               

            }
            else if (fieldLabel.Equals(FieldGroupOrFieldLabel.Group))
            { 
                var handle = await elementHandle.QuerySelectorAsync($"{CssSelectorClass.LegendSelector}");
                fieldName = await handle.EvaluateFunctionAsync<string>("el => el.innerText");
                //TODO: handle attachment separately. For now since it will not be required field we can ignore
                if (fieldName.EqualsWithIgnoreCase("Attachment"))
                {
                    fieldId = "attachment";
                   // return (fieldId, fieldName, ElementType.FileUpload, new List<string>());
                }
                else
                {
                    //TODO: check other differnt group scenarios
                    fieldId = await page.EvaluateFunctionAsync<string>($"x=>x.getAttribute('{CssSelectorClass.CheckboxSelector}')", elementHandle);
                    if (fieldId != null)
                    {
                        checkBox = true;
                    }
                }

            }
            else
            {
                var message = $"No label found for {await GetHandleString(elementHandle)}";
                businessExceptionFactory.AddBusinessException(new BusinessException(SeverityType.Critical, message,
                    dto, TestType.Screen));

                
            }

            if (!string.IsNullOrEmpty(fieldId))
            {
                var elementTypeWithValues = await GetElementType(dto, page, elementHandle, fieldId, fieldLabel, checkBox);
                elementType = elementTypeWithValues.elementType;
                lstAvailableValues = elementTypeWithValues.lstAvailableValues.ToList();
            }

            return (fieldId, fieldName, elementType, lstAvailableValues);

        }

        private async Task<bool> IsRequired(IPage page, IElementHandle elementHandle)
        {
            var handle = await elementHandle.QuerySelectorAsync($"{CssSelectorClass.RequiredSelector}");

            return handle != null;
        }

        private async Task<string> GetHandleString(IElementHandle elementHandle)
        {
            return  await elementHandle.EvaluateFunctionAsync<string>("el => el.innerHtml");
        }
    }
}
