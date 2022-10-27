

namespace JiraTesterProService.FileHandler
{
    public class FileFactory : IFileFactory
    {
        private IFileHandlerFactory fileHandlerFactory;
        private ILogger<FileFactory> logger;
        public FileFactory(IFileHandlerFactory fileHandlerFactory, ILogger<FileFactory> logger)
        {
            this.fileHandlerFactory = fileHandlerFactory;
            this.logger = logger;
        }

        public async Task<DataTable> GetDataTableFromFile(FileInfo file)
        {
            var fileService = fileHandlerFactory.GetFileService(file.Extension, file.Name);
            DataTable datatable = new DataTable();
            using (var stream = new MemoryStream())
            {

                using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    // Here we're telling the FileEdit where to write the upload result
                    await fs.CopyToAsync(stream);
                    // Once we reach this line it means the file is fully uploaded.

                    // In this case we're going to offset to the beginning of file
                    // so we can read it.
                    stream.Seek(0, SeekOrigin.Begin);
                    datatable = fileService.GetDataTableFromFile(stream);
                    datatable.TableName = TableNameUtil.GetDataTableName(new ReconConfig(), file.Name, string.Empty);
                }

            }

            return datatable;
        }


       
    }
}
