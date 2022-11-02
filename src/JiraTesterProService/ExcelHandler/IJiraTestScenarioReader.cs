namespace JiraTesterProService.ExcelHandler;

public interface IJiraTestScenarioReader
{
    Task<IList<JiraTestMasterDto>> GetJiraMasterDtoFromMatrix(string path);
}