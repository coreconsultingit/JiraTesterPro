using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorDownloadFile;
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
    public class IndexBase : ComponentBase
    {

        [Inject] protected ILogger<IndexBase> Logger { get; set; }
        protected string OutputFilePath { get; set; } = @"C:\temp1\";
        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] protected IJiraFileConfigProvider JiraFileConfigProvider { get; set; }
        protected IBrowserFile file;
        private long maxFileSize = 1024 * 1024;
        protected bool Isloading { get; set; } = false;
        [Inject] protected IBlazorDownloadFileService BlazorDownloadFileService { get; set; }

        [Inject] protected IJiraTestWorkflowRunner JiraTestWorkflowRunner { get; set; }
        

        protected JiraWorkFlowResult result;
        protected string ErrorText = String.Empty;
        protected override Task OnInitializedAsync()

        {
            var options = new DialogOptions
                { CloseButton = false, CloseOnEscapeKey = false, DisableBackdropClick = true };
            DialogService.Show<Login>("Login", options);


           
            return Task.CompletedTask;
        }
     

        protected  string RowStyleFunc(JiraTestResultWriterResult arg1, int index)
        {
            //switch (arg1.Title)
            //{
            //    case string a when a.Contains("1/4"):
            //        return "background-color:blue";
            //    case string b when b.Contains("2/4"):
            //        return "background-color:red";
            //    default: return "background-color:white";

            //}
            return arg1.CssClass;
        }

        protected async Task UploadFiles(InputFileChangeEventArgs e)
        {
            try
            {
                ErrorText = String.Empty;
                Isloading = true;
                file = e.File;
                if (!Directory.Exists(OutputFilePath))
                {
                    Logger.LogInformation($"Directory doesn't exist so creating it{OutputFilePath}");
                    Directory.CreateDirectory(OutputFilePath);
                }

                var masterTestFile = $"{OutputFilePath}\\{e.File.Name}";
                await using FileStream fs = new(masterTestFile, FileMode.Create);
                await file.OpenReadStream(maxFileSize).CopyToAsync(fs);
                fs.Close();

                JiraFileConfigProvider.InitializeConfig(new FileConfigDto()
                {
                    MasterTestFile = masterTestFile,
                    OutputJiraTestFilePath = OutputFilePath
                });

                result = await JiraTestWorkflowRunner.RunJiraWorkflow();


                Isloading = false;


            }
            catch (Exception ex)
            {
                Isloading = false;
                Logger.LogError(ex.Message);
                ErrorText = ex.Message;

            }

        }

        protected async Task DownLoad(string fileName)
        {
            FileInfo fs = new FileInfo(fileName);
            await BlazorDownloadFileService.DownloadFile(fs.Name, File.ReadAllBytes(fileName), "application/octet-stream");
        }
    }
}
