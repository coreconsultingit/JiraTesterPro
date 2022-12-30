using JiraTesterProData.JiraMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraTesterProService.JiraHtmlHelper
{
    public interface IJiraScreenTestComparer
    {
        IList<ScreenTestResult> CompareScreenFields(IList<JiraHtmlFieldDto> lstJiraHtmlFields, JiraTestMasterDto dto);

    }
}
