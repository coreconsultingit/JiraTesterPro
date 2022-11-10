using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraTesterProData;
using JiraTesterProService.JiraParser;
using JiraTesterProService.Workflow;
using JiraTestProUI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using MudBlazor;
using Serilog.Core;

namespace JiraTestProUI
{
    public class IndexBase:ComponentBase
    {

        [Inject]
        protected ILogger<IndexBase> Logger { get; set; }
        protected string OutputFilePath { get; set; } = @"C:\temp1\";
        [Inject]
        protected IDialogService DialogService { get; set; }
        [Inject]
        protected IJiraFileConfigProvider JiraFileConfigProvider { get; set; }
        protected IBrowserFile file ;
        private long maxFileSize = 1024 * 1024;
        protected bool Isloading { get; set; } = false;
        [Inject]
        protected IJiraTestWorkflowRunner JiraTestWorkflowRunner { get; set; }

        protected IList<JiraTestResult> lstJiraTestResults;
        protected override async Task OnInitializedAsync()

        {
            var options = new DialogOptions { CloseButton = false, CloseOnEscapeKey = false, DisableBackdropClick = true }; 
            DialogService.Show<Login>("Login", options);
        }

        protected async Task UploadFiles(InputFileChangeEventArgs e)
        {
            try
            {

                Isloading = true;
                file = e.File;
                if (!Directory.Exists(OutputFilePath))
                {
                    Logger.LogInformation($"Directory doesn't exist so creating it{OutputFilePath}");
                    Directory.CreateDirectory(OutputFilePath);
                }
                var masterTestFile= $"{OutputFilePath}\\{e.File.Name}";
                await using FileStream fs = new(masterTestFile, FileMode.Create);
                await file.OpenReadStream(maxFileSize).CopyToAsync(fs);
                fs.Close();
                
                JiraFileConfigProvider.InitializeConfig(new FileConfigDto()
                {
                    MasterTestFile = masterTestFile,
                    OutputJiraTestFilePath = OutputFilePath
                });


                lstJiraTestResults=await JiraTestWorkflowRunner.RunJiraWorkflow();
                Isloading = false;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw;
            }

        }
    }
}
