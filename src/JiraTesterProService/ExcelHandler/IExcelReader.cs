using System.Data;

namespace JiraTesterProService.ExcelHandler
{
    public interface IExcelReader
    {
        DataTable GetExcelData(MemoryStream memoryStream, bool hasHeader = true, string worksheet = null);
        IList<DataTable> GetMultiExcelData(MemoryStream memoryStream, bool hasHeader = true);
    }
}
