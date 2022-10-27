namespace JiraTesterProService.FileHandler
{
    public class FileHandlerFactory : IFileHandlerFactory
    {
        private readonly IServiceProvider serviceProvider;

        public FileHandlerFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IFileService GetFileService(string fileExtension, string fileName)
        {
            var fileNameextension = fileName.Substring(fileName.LastIndexOf('.') + 1).ToUpper();
            if (fileNameextension == "CSV")
            {
                return (IFileService)serviceProvider.GetService(typeof(DelimettedFileService));
            }

            var extension = fileExtension.ToUpper().Substring(fileExtension.LastIndexOf('.') + 1);

            switch (extension)
            {
                case "CSV":
                case "TXT":
                case "TEXT/PLAIN":
                    return (IFileService)serviceProvider.GetService(typeof(DelimettedFileService));
                case "XLSX":
                case "SHEET":
                    return (IFileService)serviceProvider.GetService(typeof(ExcelFileService));
                default:
                    throw new Exception("Not supported file type");


            }
        }
    }
}
