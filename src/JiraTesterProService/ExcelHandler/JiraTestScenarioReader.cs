using JiraTesterProData.Extensions;
using System.IO;
using System.Text;
using JiraTesterProData.JiraMapper;
using JiraTesterProService.JiraParser;

namespace JiraTesterProService.ExcelHandler;

public class JiraTestScenarioReader : IJiraTestScenarioReader
{
    private ILogger<JiraTestScenarioReader> logger;
    private IJiraCustomParser jiraCustomParser;
    private IDictionary<string, JiraRootobject> dictProjectWithJira = new Dictionary<string, JiraRootobject>();
    private IDictionary<string, JiraIssuetype> dictProjectWithIssueTypeJira = new Dictionary<string, JiraIssuetype>();
    public JiraTestScenarioReader(ILogger<JiraTestScenarioReader> logger, IJiraCustomParser jiraCustomParser)
    {
        this.logger = logger;
        this.jiraCustomParser = jiraCustomParser;
    }

    public  async Task<IList<JiraTestMasterDto>> GetJiraMasterDtoFromMatrix(string path)
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

                    var dictTestCell = new Dictionary<string, int>();
                    DataTable tbl = new DataTable();
                    var columnlist = new List<string>();

                    var projectCode = worksheet.Cells[celladdress.Start.Row, celladdress.Start.Column+1].Value;

                    if (projectCode == null)
                    {
                        logger.LogError($"No project code found at row {celladdress.Start.Row} column {celladdress.Start.Column + 1}");
                    }

                    var issueType = worksheet.Cells[celladdress.Start.Row+1, celladdress.Start.Column + 1].Value;

                    if (issueType == null)
                    {
                        logger.LogError($"No issue type found at row {celladdress.Start.Row+1} column {celladdress.Start.Column + 1}");
                    }


                    var projectCodeVal = projectCode.GetNoneIfEmptyOrNull();

                    if (!dictProjectWithJira.ContainsKey(projectCodeVal))
                    {
                        dictProjectWithJira.Add(projectCodeVal, await jiraCustomParser.GetParsedJiraRootBasedOnProject(projectCodeVal));
                    }

                    var groupCode = worksheet.Cells[celladdress.Start.Row+2, celladdress.Start.Column].Value;
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
                    for (int rowNum = celladdress.Start.Row + 4; rowNum <= worksheet.Dimension.End.Row; rowNum++)
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


                    for (int i = 0; i < tbl.Rows.Count; i++)
                    {
                        
                        dictTestCell.Add(tbl.Rows[i].ItemArray[0].GetNoneIfEmptyOrNull(),i);
                    }

                    for (int i=1; i< tbl.Columns.Count;i++)
                    {
                        var test = new JiraTestMasterDto()
                        {
                            StepId = i,
                            Project = projectCode.GetNoneIfEmptyOrNull(),
                            GroupKey = groupCode.GetNoneIfEmptyOrNull(),
                            OrderId = i,
                            IssueType = issueType.GetNoneIfEmptyOrNull(),
                            Action = GetAction(tbl.Rows[dictTestCell["Button (Transition)"]].ItemArray[i].GetNoneIfEmptyOrNull()),
                            ExpectedStatus = tbl.Rows[dictTestCell["Resulting Status"]].ItemArray[i].GetNoneIfEmptyOrNull(),
                            Expectation = JiraTestStatusEnum.Passed.ToString(),
                            Status = tbl.Rows[dictTestCell["Button (Transition)"]].ItemArray[i].GetNoneIfEmptyOrNull(),
                            Scenario = tbl.Rows[dictTestCell["Scenario/Step"]].ItemArray[i].GetNoneIfEmptyOrNull()
                        };

                        PopulateRequiredFields(test);

                        lstJiraMasterDto.Add(test);
                    }
                } }

          
        }

        return lstJiraMasterDto;
    }


    public void PopulateRequiredFields(JiraTestMasterDto dto)
    {
        var root = dictProjectWithJira[dto.Project];
        var key = $"{dto.Project}_{dto.IssueType}";
        if (!dictProjectWithIssueTypeJira.ContainsKey(key))
        {
            var field = root.projects[0].issuetypes.Where(x => x.name.EqualsWithIgnoreCase(dto.IssueType))
                .FirstOrDefault();
            if (field == null)
            {
                logger.LogError($"Issue type {dto.IssueType} not found for {dto.Project}");
                return;
            }
            
            dictProjectWithIssueTypeJira.Add(key, field);
        }


        //Summary
        var fielddefinition = dictProjectWithIssueTypeJira[key].fields;
        if (fielddefinition.summary.required)
        {
            dto.Summary = "Test Summary";
        }

        if (fielddefinition.components.required)
        {
            dto.Component = fielddefinition.components.allowedValues[0].name;
        }

        var customFieldinput = new List<string>();
        foreach (var customfield in fielddefinition.Customfield)
        {
            if (customfield.required && !customfield.hasDefaultValue)
            {
                if (customfield.allowedValues.Any())
                {
                    customFieldinput.Add($"{customfield.name}:{customfield.allowedValues[0].value}");
                }
                else
                {
                    customFieldinput.Add($"{customfield.name}:Test");
                }
            }
        }
        dto.CustomFieldInput = string.Join("|",customFieldinput);
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