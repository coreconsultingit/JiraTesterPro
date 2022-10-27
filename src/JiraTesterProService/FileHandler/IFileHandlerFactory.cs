namespace JiraTesterProService.FileHandler
{
    public interface IFileHandlerFactory
    {
        IFileService GetFileService(string fileExtension, string fileName);
    }
}
