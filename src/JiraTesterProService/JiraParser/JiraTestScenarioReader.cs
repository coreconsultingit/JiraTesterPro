using System.Diagnostics;
using JiraTesterProData.Extensions;
using System.IO;
using System.Text;
using JiraTesterProData.JiraMapper;
using JiraTesterProService.ExcelHandler;
using Exception = System.Exception;
using System;

namespace JiraTesterProService.JiraParser;

public class JiraTestScenarioReader : IJiraTestScenarioReader
{
    private ILogger<JiraTestScenarioReader> logger;
    
    private IDataTableParser<ScreenTestDto> screenTestDataTableParser;

    private IList<string> lstKeyDictionaryValue = new List<string>()
    {
        "Button (Transition)", "Resulting Status", "Scenario/Step"
    };

    private HashSet<string> ScreenTestList = new HashSet<string>();
    private HashSet<Guid> UniqueTestList = new HashSet<Guid>();
    private HashSet<string> GroupKeyList = new HashSet<string>();
    public JiraTestScenarioReader(ILogger<JiraTestScenarioReader> logger, IDataTableParser<ScreenTestDto> screenTestDataTableParser)
    {
        this.logger = logger;
        
        this.screenTestDataTableParser = screenTestDataTableParser;
    }

    public Task<IList<JiraTestMasterDto>> GetJiraMasterDtoFromMatrix(string path)
    {


        var lstJiraMasterDto = new List<JiraTestMasterDto>();
        FileInfo fi = new FileInfo(path);
        try
        {
            using (var package = new ExcelPackage(fi.Open(FileMode.Open)))
            {
                var workbook = package.Workbook;
                var workbookName = fi.Name.Replace(" ", "").Replace(".", "");
                if (workbookName.Length > 50)
                {
                    workbookName = workbookName.Substring(0, 49);
                }

                int iStepId = 1;
                int iGroupKey = 0;
                foreach (var worksheet in workbook.Worksheets.Where(x => !x.Name.ContainsWithIgnoreCase("Screen")))
                {
                    int rowStart = worksheet.Dimension.Start.Row;
                    int rowEnd = worksheet.Dimension.End.Row;
                    string cellRange = rowStart.ToString() + ":" + rowEnd.ToString();
                    var searchCell = from cell in worksheet.Cells[cellRange] //you can define your own range of cells for lookup
                                     where cell.Value.GetNoneIfEmptyOrNull().EqualsWithIgnoreCase("ProjectCode")
                                     select cell;

                   try
                    {
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
                            if (GroupKeyList.Contains(groupCode))
                            {
                                groupCode = $"{groupCode}_{iGroupKey}";
                                iGroupKey += 1;
                            }
                            
                            var uniqueKey = DateTime.Now.GenerateGuid(groupCode.ToString());
                           
                            
                            if (!UniqueTestList.Contains(uniqueKey))
                            {
                                UniqueTestList.Add(uniqueKey);

                            }
                            else
                            {
                                uniqueKey = DateTime.Now.GenerateGuid(groupCode.ToString() + "dupe");
                                UniqueTestList.Add(uniqueKey);
                            }
                            logger.LogInformation($"Started to created the test for group key {groupCode}");

                            var tbl = ReadTableContent(worksheet, celladdress.Start.Row + 2);

                            for (int i = 0; i < tbl.Rows.Count; i++)
                            {

                                dictTestCell.Add(tbl.Rows[i].ItemArray[0].GetNoneIfEmptyOrNull(), i);
                            }

                            for (int i = 1; i < tbl.Columns.Count; i++)
                            {

                                try
                                {
                                    var suppliedValue = GetSuppliedValues(i, tbl);
                                    if (!suppliedValue.Any())
                                    {
                                        continue;
                                    }

                                    var missingKeys = lstKeyDictionaryValue.Except(suppliedValue.Keys);
                                    if (missingKeys.Any())
                                    {
                                        logger.LogError($"Missing key values {string.Join(", ", missingKeys)}");
                                        throw new InvalidDataException();
                                    }
                                    var test = new JiraTestMasterDto()
                                    {
                                        StepId = iStepId,
                                        Project = projectCode.GetNoneIfEmptyOrNull().Replace("\n", "").Trim(),
                                        GroupKey = groupCode.GetNoneIfEmptyOrNull().Trim(),
                                        OrderId = i,
                                        IssueType = issueType.GetNoneIfEmptyOrNull().Replace("\n", "").Trim(),
                                        Action = GetAction(suppliedValue["Button (Transition)"]),
                                        ExpectedStatus = suppliedValue["Resulting Status"],
                                        Expectation = JiraTestStatusConst.Passed.ToString(),
                                        Status = suppliedValue["Button (Transition)"],
                                        Scenario = suppliedValue["Scenario/Step"],
                                        FileName = $"{workbookName}_{worksheet.Name.Replace(" ","").Replace(".","")}",
                                        SuppliedValues = suppliedValue,
                                        UniqueKey = uniqueKey.ToString()

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
                                    else if (ScreenTestList.Contains(screenworksheetName))
                                    {
                                        logger.LogInformation($"{screenworksheetName} has already been attached to the previous test case");
                                    }
                                    else
                                    {
                                        var screenTestTable = ReadTableContent(screenworksheet, 1);
                                        var parsedScreenTest =
                                            screenTestDataTableParser.ConvertDataTableToList(screenTestTable, null);

                                        if (parsedScreenTest.lstValidationMessage.Any())
                                        {
                                            logger.LogError(string.Join(", ", parsedScreenTest.lstValidationMessage));
                                            throw new Exception($"{screenworksheetName} has invalid screentest format");
                                        }
                                        else
                                        {
                                            test.ScreenTestDto = parsedScreenTest.lstItems;
                                            ScreenTestList.Add(screenworksheetName);
                                        }

                                    }

                                    lstJiraMasterDto.Add(test);
                                }
                                catch (Exception e)
                                {
                                   logger.LogError(e.Message);
                                   throw;
                                }
                                
                                iStepId += 1;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Failed on stepId {iStepId}");
                        logger.LogError(ex.Message);
                        throw;
                    }

                }
            }

            return Task.FromResult<IList<JiraTestMasterDto>>(lstJiraMasterDto);
        }
        catch (Exception e)
        {
           logger.LogError(e.Message);
            throw;
        }

        
    }

    private IDictionary<string, string> GetSuppliedValues(int iColumnIndex,DataTable dt)
    {
        var dictValues = new Dictionary<string, string>();
        foreach (DataRow row in dt.Rows)
        {
            var key = row.ItemArray[0].GetEmptyIfEmptyOrNull().Trim();
            if (string.IsNullOrEmpty(key))
            {
                continue;
            }
            if (!dictValues.ContainsKey(key))
            {
                var definedValue = row.ItemArray[iColumnIndex].GetEmptyIfEmptyOrNull();
                if (!string.IsNullOrEmpty(definedValue))
                {
                    dictValues.Add(key, definedValue);
                }
                
            }
            else
            {
                logger.LogError($"Duplicate {key} found");
            }
            


        }

        return dictValues;

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