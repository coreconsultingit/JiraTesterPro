using JiraTesterProData.JiraMapper;
using PuppeteerSharp;

namespace JiraTesterProService.JiraHtmlHelper;

public interface IJiraHtmlFieldDtoFactory
{
    Task<JiraHtmlFieldDto> GetJiraHtmlFieldDto(JiraTestMasterDto dto, IPage page, IElementHandle elementHandle);
}