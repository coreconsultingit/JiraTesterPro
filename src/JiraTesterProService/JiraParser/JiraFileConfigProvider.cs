using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraTesterProService.JiraParser
{

    public interface IJiraFileConfigProvider
    {
        string OutputJiraTestFilePathWithMasterFile { get; }

        (string outputfile, string zipfilepath) OutputJiraTestFilePathWithMasterFileByGroupKey(string key);

        string OutputJiraTestFilePathWithMaster { get; }

        void InitializeConfig(FileConfigDto fileConfigDto);
        FileConfigDto GetFileConfigDto();
    }

    //TODo: refactor this clas better
    public class JiraFileConfigProvider: IJiraFileConfigProvider
    {
        
        private ILogger<JiraFileConfigProvider> logger;

        private IMemoryCache memoryCache;
        public JiraFileConfigProvider( ILogger<JiraFileConfigProvider> logger, IMemoryCache memoryCache)
        {
            
            this.logger = logger;
            this.memoryCache = memoryCache;
        }

        public void InitializeConfig(FileConfigDto fileConfigDto)
        {
            memoryCache.Set(CacheConst.FileConfig, fileConfigDto);
        }

        public FileConfigDto GetFileConfigDto()
        {
            memoryCache.TryGetValue(CacheConst.FileConfig, out FileConfigDto fileConfigDto);
            if (fileConfigDto == null)
            {
                logger.LogError("File Config not initialized");
                throw new Exception("File Config not initialized");
            }

            return fileConfigDto;
        }


        public (string outputfile, string zipfilepath) OutputJiraTestFilePathWithMasterFileByGroupKey(string key)
        {
            var masterfile = new FileInfo(OutputJiraTestFilePathWithMaster);
            memoryCache.TryGetValue(CacheConst.FileConfig, out FileConfigDto fileConfigDto);
            if (fileConfigDto == null)
            {
                logger.LogError("File Config not initialized");
                throw new Exception("File Config not initialized");
            }
            var dir = new DirectoryInfo(fileConfigDto.OutputJiraTestFilePath);
            return (System.IO.Path.Combine(dir.FullName, Path.GetFileNameWithoutExtension(masterfile.Name).StandardiseColumnTableName(), DateTime.Now.ToString("yyyyMMdd"), key, $"TestOutPut.html"),
                System.IO.Path.Combine(dir.FullName, Path.GetFileNameWithoutExtension(masterfile.Name).StandardiseColumnTableName(), DateTime.Now.ToString("yyyyMMdd")));
        }

        public string OutputJiraTestFilePathWithMaster
        {
            get
            {

                memoryCache.TryGetValue(CacheConst.FileConfig, out FileConfigDto fileConfigDto);
                if (fileConfigDto == null)
                {
                    logger.LogError("File Config not initialized");
                    throw new Exception("File Config not initialized");
                }
                var masterfile = new FileInfo(fileConfigDto.MasterTestFile);
                var dir = new DirectoryInfo(fileConfigDto.OutputJiraTestFilePath);
                var fullpath = System.IO.Path.Combine(dir.FullName, Path.GetFileNameWithoutExtension(masterfile.Name).StandardiseColumnTableName());
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
                memoryCache.TryGetValue(CacheConst.FileConfig, out FileConfigDto fileConfigDto);
                if (fileConfigDto == null)
                {
                    logger.LogError("File Config not initialized");
                    throw new Exception("File Config not initialized");
                }
                var dir = new DirectoryInfo(fileConfigDto.OutputJiraTestFilePath);
                return System.IO.Path.Combine(dir.FullName, Path.GetFileNameWithoutExtension(masterfile.Name).StandardiseColumnTableName(), DateTime.Now.ToString("yyyyMMdd"), $"TestOutPut.html");


            }
        }

    }
}
