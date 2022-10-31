using System.IO;

namespace JiraTesterProService.ExcelHandler;

public class JiraTestScenarioReader : IJiraTestScenarioReader
{
    private ILogger<JiraTestScenarioReader> logger;
    public JiraTestScenarioReader(ILogger<JiraTestScenarioReader> logger)
    {
        this.logger = logger;
    }

    public IList<JiraTestMasterDto> GetJiraMasterDtoFromMatrix(string path)
    {
        var lstJiraMasterDto = new List<JiraTestMasterDto>();
        using (var package = new ExcelPackage(File.Open(path,FileMode.Open)))
        {
            var workbook = package.Workbook;
            foreach (var worksheet in workbook.Worksheets)
            {
                int rowStart = worksheet.Dimension.Start.Row;
                int rowEnd = worksheet.Dimension.End.Row;
                string cellRange = rowStart.ToString() + ":" + rowEnd.ToString();
                var searchCell = from cell in worksheet.Cells[cellRange] //you can define your own range of cells for lookup
                    where cell.Value.GetNoneIfEmptyOrNull().EqualsWithIgnoreCase("ProjectCode")
                    select cell;


                foreach (var celladdress in searchCell)
                {
                    DataTable tbl = new DataTable();
                    var columnlist = new List<string>();

                    var projectCode = worksheet.Cells[celladdress.Start.Row, celladdress.Start.Column+1].Value;
                    var groupCode = worksheet.Cells[celladdress.Start.Row+1, celladdress.Start.Column].Value;
                    int iCounter = 1;
                    foreach (var firstRowCell in worksheet.Cells[celladdress.Start.Row+2, 1, celladdress.Start.Row+2, worksheet.Dimension.End.Column])
                    {
                        var columnname = firstRowCell.Text.StandardiseColumnTableName();
                        if (columnlist.Contains(columnname))
                        {
                            columnname += iCounter.ToString();
                            iCounter += 1;
                        }
                        tbl.Columns.Add(columnname);
                        columnlist.Add(columnname);
                    }
                    for (int rowNum = 4; rowNum <= worksheet.Dimension.End.Row; rowNum++)
                    {
                        var endcolumn = worksheet.Dimension.End.Column;
                        var wsRow = worksheet.Cells[rowNum, 1, rowNum, endcolumn];
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

                    for (int i=1; i< tbl.Columns.Count;i++)
                    {
                        var test = new JiraTestMasterDto()
                        {
                            Project = projectCode.GetNoneIfEmptyOrNull(),
                            GroupKey = groupCode.GetNoneIfEmptyOrNull()
                        };

                        lstJiraMasterDto.Add(test);
                    }
                }


               

            }

          
        }

        return lstJiraMasterDto;
    }
}