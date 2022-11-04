namespace JiraTesterProService.JiraParser;

public interface IJiraTestScenarioReader
{
    Task<IList<JiraTestMasterDto>> GetJiraMasterDtoFromMatrix(string path);
}