using JiraTesterProData.Extensions;

namespace JiraTesterProData;

public class ScreenTestDto
{
    public int SlNo { get; set; } = 0;
    public string FieldName { get; set; } = string.Empty;
    public string SystemField { get; set; } = string.Empty;
    public string ListValuesAvailable { get; set; } = string.Empty;
    public string Mandatory { get; set; } = string.Empty;
    public bool IsMandatory => Mandatory.EqualsWithIgnoreCase("YES");
}