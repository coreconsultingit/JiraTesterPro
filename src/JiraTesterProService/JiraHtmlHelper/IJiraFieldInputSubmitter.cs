using JiraTesterProData.JiraMapper;
using Microsoft.Extensions.Configuration;
using PuppeteerSharp;

namespace JiraTesterProService.JiraHtmlHelper;

public interface IJiraFieldInputSimulator
{
    Task<JiraHtmlFieldDto> SimulateInput(IPage page, IElementHandle elementHandle, JiraHtmlFieldDto fielddto,
        JiraTestMasterDto dto);
}