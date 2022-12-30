using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorDownloadFile;
using Microsoft.AspNetCore.Components;

namespace JiraTestProUI.Shared
{
    public class MainLayoutBase : LayoutComponentBase
    {
        protected bool isVisible;
        protected bool _drawerOpen = true;
        [Inject] protected IBlazorDownloadFileService BlazorDownloadFileService { get; set; }
        protected void OpenOverlay()
        {
            isVisible = true;
            StateHasChanged();
        }

        protected void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }
        protected async Task DownLoadUserManual()
        {
            FileInfo fs = new FileInfo("wwwroot\\UserManual.docx");
            await BlazorDownloadFileService.DownloadFile(fs.Name, File.ReadAllBytes(fs.FullName), "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }
    }
}
