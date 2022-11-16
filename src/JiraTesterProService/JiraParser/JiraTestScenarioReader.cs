using JiraTesterProData.Extensions;
using System.IO;
using System.Text;
using JiraTesterProData.JiraMapper;
using JiraTesterProService.ExcelHandler;

namespace JiraTesterProService.JiraParser;

public class JiraTestScenarioReader : IJiraTestScenarioReader
{
    private ILogger<JiraTestScenarioReader> logger;
    
    private IDataTableParser<ScreenTestDto> screenTestDataTableParser;
    public JiraTestScenarioReader(ILogger<JiraTestScenarioReader> logger, IDataTableParser<ScreenTestDto> screenTestDataTableParser)
    {
        this.logger = logger;
        
        this.screenTestDataTableParser = screenTestDataTableParser;
    }

    public async Task<IList<JiraTestMasterDto>> GetJiraMasterDtoFromMatrix(string path)
    {


        var lstJiraMasterDto = new List<JiraTestMasterDto>();
        FileInfo fi = new FileInfo(path);
        try
        {
            using (var package = new ExcelPackage(fi.Open(FileMode.Open)))
            {
                var workbook = package.Workbook;

                int iStepId = 0;
                foreach (var worksheet in workbook.Worksheets.Where(x => x.Name.ContainsWithIgnoreCase("WorkFlow")))
                {
                    int rowStart = worksheet.Dimension.Start.Row;
                    int rowEnd = worksheet.Dimension.End.Row;
                    string cellRange = rowStart.ToString() + ":" + rowEnd.ToString();
                    var searchCell = from cell in worksheet.Cells[cellRange] //you can define your own range of cells for lookup
                                     where cell.Value.GetNoneIfEmptyOrNull().EqualsWithIgnoreCase("ProjectCode")
                                     select cell;


                    foreach (var celladdress in searchCell)
                    {

                        var dictTestCell = new Dictionary<string, int>();
                        

                        var projectCode = worksheet.Cells[celladdress.Start.Row, celladdress.Start.Column + 1].Value;

                        if (projectCode == null)
                        {
                            logger.LogError($"No project code found at row {celladdress.Start.Row} column {celladdress.Start.Column + 1}");
                        }

                        var issueType = worksheet.Cells[celladdress.Start.Row + 1, celladdress.Start.Column + 1].Value;

                        if (issueType == null)
                        {
                            logger.LogError($"No issue type found at row {celladdress.Start.Row + 1} column {celladdress.Start.Column + 1}");
                        }


                       

                        var groupCode = worksheet.Cells[celladdress.Start.Row + 2, celladdress.Start.Column].Value;

                        var tbl = ReadTableContent(worksheet, celladdress.Start.Row + 2);

                        for (int i = 0; i < tbl.Rows.Count; i++)
                        {

                            dictTestCell.Add(tbl.Rows[i].ItemArray[0].GetNoneIfEmptyOrNull(), i);
                        }

                        for (int i = 1; i < tbl.Columns.Count; i++)
                        {
                            var test = new JiraTestMasterDto()
                            {
                                StepId = iStepId,
                                Project = projectCode.GetNoneIfEmptyOrNull(),
                                GroupKey = groupCode.GetNoneIfEmptyOrNull(),
                                OrderId = i,
                                IssueType = issueType.GetNoneIfEmptyOrNull(),
                                Action = GetAction(tbl.Rows[dictTestCell["Button (Transition)"]].ItemArray[i].GetNoneIfEmptyOrNull()),
                                ExpectedStatus = tbl.Rows[dictTestCell["Resulting Status"]].ItemArray[i].GetNoneIfEmptyOrNull(),
                                Expectation = JiraTestStatusEnum.Passed.ToString(),
                                Status = tbl.Rows[dictTestCell["Button (Transition)"]].ItemArray[i].GetNoneIfEmptyOrNull(),
                                Scenario = tbl.Rows[dictTestCell["Scenario/Step"]].ItemArray[i].GetNoneIfEmptyOrNull(),
                                FileName = $"{fi.Name}_{worksheet.Name}"
                            };

                           // PopulateRequiredFields(test);
                            //Read the Screenscenario
                            var screenworksheetName = $"{issueType.GetNoneIfEmptyOrNull()} - {test.Status} Screen";
                            var screenworksheet =
                                workbook.Worksheets.FirstOrDefault(x => x.Name.EqualsWithIgnoreCase(screenworksheetName));
                            if (screenworksheet == null)
                            {
                                logger.LogInformation($"No screen details found for this status {screenworksheetName}");
                            }
                            else
                            {
                                var screenTestTable = ReadTableContent(screenworksheet, 1);
                                var parsedScreenTest =
                                    screenTestDataTableParser.ConvertDataTableToList(screenTestTable, null);

                                if (parsedScreenTest.lstValidationMessage.Any())
                                {
                                    logger.LogError(string.Join(", ",parsedScreenTest.lstValidationMessage));
                                    throw new Exception($"{screenworksheetName} has invalid screentest format");
                                }
                                else
                                {
                                    test.ScreenTestDto = parsedScreenTest.lstItems;
                                }


                            }

                            lstJiraMasterDto.Add(test);
                            iStepId = iStepId + 1;

                        }

                        
                    }
                }
            }

            return lstJiraMasterDto;
        }
        catch (Exception e)
        {
           logger.LogError(e.Message);
            throw;
        }

        
    }
    private DataTable ReadColum(ExcelWorksheet worksheet, int startRow)
    {
        var columnlist = new List<string>();
        DataTable tbl = new DataTable();
        int iCounter = 1;
        foreach (var firstRowCell in worksheet.Cells[startRow, 1, startRow,
                     worksheet.Dimension.End.Column])
        {
            var columnname = firstRowCell.Text.StandardiseColumnTableName();
            if (string.IsNullOrEmpty(columnname))
            {
                columnname = iCounter.ToString();
            }
            if (columnlist.Contains(columnname))
            {
                columnname += iCounter.ToString();
                iCounter += 1;
            }

            tbl.Columns.Add(columnname);
            columnlist.Add(columnname);
        }

        return tbl;
    }
    private DataTable ReadTableContent(ExcelWorksheet worksheet, int startRow)
    {
        var tbl = ReadColum(worksheet, startRow);
        for (int rowNum = startRow + 1; rowNum <= worksheet.Dimension.End.Row; rowNum++)
        {
            var endcolumn = worksheet.Dimension.End.Column;
            var wsRow = worksheet.Cells[rowNum, 1, rowNum, endcolumn];
            if (wsRow.All(c => c.Value == null))
            {
                break;
            }

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


    

    private string GetAction(string val)
    {
        if (val.ContainsWithIgnoreCase("CREATE"))
        {
            return JiraActionEnum.Create.ToString();
        }

        return JiraActionEnum.Update.ToString();
    }
}