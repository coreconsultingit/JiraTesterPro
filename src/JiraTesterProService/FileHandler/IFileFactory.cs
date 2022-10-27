namespace JiraTesterProService.FileHandler
{
    public interface IFileFactory
    {
       

        Task<DataTable> GetDataTableFromFile(FileInfo file);
    }
}
