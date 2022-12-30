namespace JiraTesterProData.Extensions;

public static class JiraFieldHelper
{
    public static IList<string> GetFieldMetaDataList()
    {
        return  new List<string>()
        {
            "Button (Transition)", "Resulting Status", "Scenario/Step", "Role","Digital Signature Point?","Pop Up Screen","Digital Signature Point","WorkFlowPath"
        };
    }
}