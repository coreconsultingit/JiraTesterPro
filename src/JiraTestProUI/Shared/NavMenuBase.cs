

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace JiraTestProUI.Shared
{
    public class NavMenuBase:ComponentBase
    {
        protected List<string> DashBoardList { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
           // var dashboards = await DashBoardDetailCacheProvider.GetDashboardDetail();
            //sourceApplications = await sourceApplicationService.GetSourceApllications();
            DashBoardList = new List<string>();

        }
    }
}
