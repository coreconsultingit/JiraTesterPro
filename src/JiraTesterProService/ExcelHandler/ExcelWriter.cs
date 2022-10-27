using System.Data;
using OfficeOpenXml;

namespace JiraTesterProService.ExcelHandler
{
    //public class ExcelWriter : IExcelWriter
    //{
    //    private IReconMessageService reconMessageService;
    //    private IReconSummaryService reconSummaryService;
    //    public ExcelWriter(IReconMessageService reconMessageService, IReconSummaryService reconSummaryService)
    //    {
    //        this.reconMessageService = reconMessageService;
    //        this.reconSummaryService = reconSummaryService;
    //    }
    //    public MemoryStream GetMemoryStreamFromExcel(IList<DataTable> lstDataTable, bool writeSummaryMessage = true, bool writereconMessage = false)
    //    {
    //        var stream = new MemoryStream();
    //        using (var package = new ExcelPackage(stream))
    //        {
    //            var reconsummaryCount = reconSummaryService.GetReconSummary().Rows.Count;
    //            if (writeSummaryMessage && reconsummaryCount > 0)
    //            {
    //                var workSheet = package.Workbook.Worksheets.Add("ReconSummary");
    //                workSheet.Cells.LoadFromDataTable(reconSummaryService.GetReconSummary(), true, OfficeOpenXml.Table.TableStyles.Medium2);

    //                //var cf = workSheet.Cells["A1:E24"].ConditionalFormatting.AddContainsText();
    //                //cf.Text = "Fail";
    //                //cf.Style.Font.Color.Color = Color.Red;
    //                ExcelAddress cfAddressStatus = new ExcelAddress($"E2:E{reconsummaryCount + 1}");
    //                var cfRule1 = workSheet.ConditionalFormatting.AddContainsText(cfAddressStatus);
    //                cfRule1.Text = "Fail";
    //                //cfRule1.Style.Fill.BackgroundColor.Color = Color.Red;
    //                cfRule1.Style.Font.Bold = true;

    //                var cfRule2 = workSheet.ConditionalFormatting.AddContainsText(cfAddressStatus);
    //                cfRule2.Text = "Pass";
    //                //cfRule2.Style.Fill.BackgroundColor.Color = Color.Green;
    //                cfRule2.Style.Font.Bold = true;


    //                reconSummaryService.ClearMessage();

    //            }

    //            foreach (var table in lstDataTable)
    //            {
    //                var workSheet = package.Workbook.Worksheets.Add(table.TableName);
    //                workSheet.Cells.LoadFromDataTable(table, true, OfficeOpenXml.Table.TableStyles.Medium2);

    //            }

    //            if (writereconMessage)
    //            {
    //                var workSheet = package.Workbook.Worksheets.Add("ReconLogMessage");
    //                workSheet.Cells.LoadFromDataTable(reconMessageService.GetReconMessage(), true, OfficeOpenXml.Table.TableStyles.Medium2);
    //                reconMessageService.ClearMessage();
    //            }

    //            package.Save();
    //        }
    //        return stream;
    //    }
    //}
}
