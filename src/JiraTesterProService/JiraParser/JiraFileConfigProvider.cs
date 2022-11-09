using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraTesterProService.JiraParser
{
    public class JiraFileConfigProvider
    {
        public string OutputJiraTestFilePath { get; private set; }

        public string MasterTestFile { get; private set; }

        public string InputJiraTestFile { get; private set; }

        private ILogger<JiraFileConfigProvider> logger;
        public JiraFileConfigProvider(string outputJiraTestFilePath, string masterTestFile, string inputJiraTestFile, ILogger<JiraFileConfigProvider> logger)
        {
            OutputJiraTestFilePath = outputJiraTestFilePath;
            MasterTestFile = masterTestFile;
            InputJiraTestFile = inputJiraTestFile;
            this.logger = logger;
        }

        public string OutputJiraTestFilePathWithMaster
        {
            get
            {
                var masterfile = new FileInfo(MasterTestFile);
                var dir = new DirectoryInfo(OutputJiraTestFilePath);
                var fullpath = System.IO.Path.Combine(dir.FullName, Path.GetFileNameWithoutExtension(masterfile.Name));
                try
                {

                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                    throw;
                }
                
                if (!Directory.Exists(fullpath))
                {
                    Directory.CreateDirectory(fullpath);
                }

                return fullpath;


            }
        }
        public string OutputJiraTestFilePathWithMasterFile
        {
            get
            {
                var masterfile = new FileInfo(OutputJiraTestFilePathWithMaster);

                var dir = new DirectoryInfo(OutputJiraTestFilePath);
                return System.IO.Path.Combine(dir.FullName, Path.GetFileNameWithoutExtension(masterfile.Name), DateTime.Now.ToString("yyyyMMdd"), $"TestOutPut.html");


            }
        }

    }
}
