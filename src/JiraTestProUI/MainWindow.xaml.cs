using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JiraTesterProService.FileHandler;
using MudBlazor.Services;
using Serilog;

namespace JiraTestProUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var logFilePath = $"{FileUtillity.AssemblyDirectory}\\..\\log\\Clarioreporterlog.txt";
            try
            {

                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                    .WriteTo.Console()
                    .CreateLogger();

                Serilog.Debugging.SelfLog.Enable(msg =>
                {
                    Debug.Print(msg);

                });

                Log.Information("Starting the app");
                var serviceCollection = new ServiceCollection();

                serviceCollection.AddWpfBlazorWebView();
#if DEBUG
                serviceCollection.AddBlazorWebViewDeveloperTools();
#endif
               // serviceCollection.RegisterDependency();
                serviceCollection.AddMudServices();

                var provider = serviceCollection.BuildServiceProvider();
                Resources.Add("services", provider);
                //var scripter = provider.GetService<TableScriptCreator>();
                //var filePath = @"C:\Users\s.kumar\Downloads\jama8741schema\schemaspy_output\jama.xml";
                //var script = scripter.GetScript(filePath);
                //var diName = new FileInfo(filePath).DirectoryName;
                //File.WriteAllText(Path.Combine(diName, "DbScript.sql"), script);


            }
            catch (Exception exception)
            {

                string text = $"Error opening the Clario Reporter app {exception.Message} {exception.InnerException}";
                File.WriteAllText(logFilePath, text);
            }
        }
    }
}
