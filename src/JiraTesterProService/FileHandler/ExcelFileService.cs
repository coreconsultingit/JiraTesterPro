using JiraTesterProService.ExcelHandler;

namespace JiraTesterProService.FileHandler
{
    public class ExcelFileService : FileServiceBase
    {
        private IExcelReader excelReader;
        public ExcelFileService(IExcelReader excelReader)
        {
            this.excelReader = excelReader;
        }
        public override DataTable GetDataTableFromFile(MemoryStream stream)
        {

            return excelReader.GetExcelData(stream);

        }
    }
}
