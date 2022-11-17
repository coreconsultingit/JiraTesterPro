using JiraTesterProData.Extensions;

namespace JiraTesterProData;

public class ScreenTestDto
{
    public int SlNo { get; set; }
    public string FieldName { get; set; }
    public string SystemField { get; set; }
    public string ListValuesAvailable { get; set; }
    public string Mandatory { get; set; }
    public bool IsMandatory => Mandatory.EqualsWithIgnoreCase("YES");
}