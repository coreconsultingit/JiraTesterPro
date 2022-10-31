


namespace JiraTesterProService.ExcelHandler
{
    public class ExcelReader : IExcelReader
    {
        private ILogger<ExcelReader> logger;
        public ExcelReader(ILogger<ExcelReader> logger)
        {
            this.logger = logger;
        }

        public DataTable GetExcelData(MemoryStream memoryStream, bool hasHeader = true, string worksheet = null)
        {
            DataTable dt = new DataTable();


            using (var package = new ExcelPackage(memoryStream))
            {
                dt = ParseSheet(package, hasHeader, worksheet);


            }

            return dt;
        }


        public IList<DataTable> GetMultiExcelData(MemoryStream memoryStream, bool hasHeader = true)
        {
            var dt = new List<DataTable>();

            using (var package = new ExcelPackage(memoryStream))
            {
                foreach (var sheet in package.Workbook.Worksheets)
                {
                    dt.Add(ParseSheet(package, hasHeader, sheet.Name));
                }


            }

            return dt;
        }

        private DataTable ParseSheet(ExcelPackage xlPackage, bool hasHeader = true, string worksheet = null)
        {
            var workbook = xlPackage.Workbook;
            ExcelWorksheet ws;
            if (string.IsNullOrEmpty(worksheet))
            {
                ws = workbook.Worksheets[0];
            }
            else
            {
                ws = workbook.Worksheets[worksheet];
            }

            DataTable tbl = new DataTable();
            var columnlist = new List<string>();

            int iCounter = 1;
            foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
            {
                var columnname = firstRowCell.Text.StandardiseColumnTableName();
                if (columnlist.Contains(columnname))
                {
                    columnname = columnname + iCounter.ToString();
                    iCounter += 1;
                }
                tbl.Columns.Add(hasHeader ? columnname : string.Format("Column {0}", firstRowCell.Start.Column));
                columnlist.Add(columnname);
            }
            var startRow = hasHeader ? 2 : 1;
            for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
            {
                var endcolumn = ws.Dimension.End.Column;
                if (ws.Name.Contains("Stock Sheet", StringComparison.OrdinalIgnoreCase))
                {
                    endcolumn = 25;

                }

                var wsRow = ws.Cells[rowNum, 1, rowNum, endcolumn];
                DataRow row = tbl.Rows.Add();
                foreach (var cell in wsRow)
                {
                    try
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e.Message + e.InnerException);

                    }

                }
            }
            return tbl;
        }
    }
}
