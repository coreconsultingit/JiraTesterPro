namespace JiraTesterProService.ExcelHandler;

public interface IJiraTestScenarioReader
{
    IList<JiraTestMasterDto> GetJiraMasterDtoFromMatrix(string path);
}