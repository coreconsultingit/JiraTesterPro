using JiraTesterProData.JiraMapper;

namespace JiraTesterProService.JiraHtmlHelper;

public interface IJiraFieldInputProvider
{
    string GetParsedFieldValue(JiraHtmlFieldDto fielddto, JiraTestMasterDto dto);
}