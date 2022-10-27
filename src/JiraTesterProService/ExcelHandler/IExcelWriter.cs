namespace JiraTesterProService.ExcelHandler
{
    public interface IExcelWriter
    {
        MemoryStream GetMemoryStreamFromExcel(IList<DataTable> lstDataTable, bool writeSummaryMessage = true, bool writereconMessage = false);
    }
}
