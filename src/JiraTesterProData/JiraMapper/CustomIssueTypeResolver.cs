using Newtonsoft.Json.Serialization;

namespace JiraTesterProData.JiraMapper;

public class CustomIssueTypeResolver : DefaultContractResolver
{
    protected override string ResolvePropertyName(string propertyName)
    {
        string resolvedName = null;
        // var resolved = this.PropertyMappings.TryGetValue(propertyName, out resolvedName);
        if (propertyName.Contains("customfield", StringComparison.OrdinalIgnoreCase))
        {
            return "Customfield";
        }
        return base.ResolvePropertyName(propertyName);
    }
}

public class JiraRootobject
{
    public string expand { get; set; }
    public JiraProject[] projects { get; set; }
}

public class JiraProject
{
    public string expand { get; set; }
    public string self { get; set; }
    public string id { get; set; }
    public string key { get; set; }
    public string name { get; set; }
    public Avatarurls avatarUrls { get; set; }
    public JiraIssuetype[] issuetypes { get; set; }
}

public class Avatarurls
{
    public string _48x48 { get; set; }
    public string _24x24 { get; set; }
    public string _16x16 { get; set; }
    public string _32x32 { get; set; }
}

public class JiraIssuetype
{
    public string self { get; set; }
    public string id { get; set; }
    public string description { get; set; }
    public string iconUrl { get; set; }
    public string name { get; set; }
    public bool subtask { get; set; }
    public string expand { get; set; }
    public JiraFields fields { get; set; }

    
}

public class JiraFields
{
    public JiraIssuetype1 issuetype { get; set; }
    public Summary summary { get; set; }
    public Components components { get; set; }
    public Assignee Assignee { get; set; }


    public IList<Customfield> Customfield { get; set; }


    public JiraFields()
    {
        Customfield = new List<Customfield>();
    }
}

public class JiraIssuetype1
{
    public bool required { get; set; }
    public JiraSchema schema { get; set; }
    public string name { get; set; }
    public string fieldId { get; set; }
    public bool hasDefaultValue { get; set; }
    public object[] operations { get; set; }
    public Allowedvalue[] allowedValues { get; set; }
}

public class JiraSchema
{
    public string type { get; set; }
    public string system { get; set; }
}

public class Allowedvalue
{
    public string self { get; set; }
    public string id { get; set; }
    public string description { get; set; }
    public string iconUrl { get; set; }
    public string name { get; set; }
    public bool subtask { get; set; }
    public int avatarId { get; set; }
}

public class Summary
{
    public bool required { get; set; }
    public JiraSchema schema { get; set; }
    public string name { get; set; }
    public string fieldId { get; set; }
    public bool hasDefaultValue { get; set; }
    public string[] operations { get; set; }
}


public class Components
{
    public bool required { get; set; }
    public Schema2 schema { get; set; }
    public string name { get; set; }
    public string fieldId { get; set; }
    public bool hasDefaultValue { get; set; }
    public string[] operations { get; set; }
    public Allowedvalue1[] allowedValues { get; set; }
}

public class Schema2
{
    public string type { get; set; }
    public string items { get; set; }
    public string system { get; set; }
}

public class Allowedvalue1
{
    public string self { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
}
public class Customfield
{

    public bool required { get; set; }

    public CustomeFieldSchema schema { get; set; }
    public string name { get; set; }
    public string fieldId { get; set; }
    public bool hasDefaultValue { get; set; }
    public string[] operations { get; set; }
    public CustomFieldAllowedvalue[] allowedValues { get; set; }
    public Defaultvalue defaultValue { get; set; }
}

public class CustomeFieldSchema
{
    public string type { get; set; }
    public string custom { get; set; }
    public int customId { get; set; }
}

public class Defaultvalue
{
    public string self { get; set; }
    public string value { get; set; }
    public string id { get; set; }
}

public class CustomFieldAllowedvalue
{
    public string self { get; set; }
    public string value { get; set; }
    public string id { get; set; }
}




public class Assignee
{
    public bool required { get; set; }
    public Assignee schema { get; set; }
    public string name { get; set; }
    public string fieldId { get; set; }
    public string autoCompleteUrl { get; set; }
    public bool hasDefaultValue { get; set; }
    public string[] operations { get; set; }
    public AssigneeDefaultvalue defaultValue { get; set; }
}

public class AssigneeSchema
{
    public string type { get; set; }
    public string system { get; set; }
}

public class AssigneeDefaultvalue
{
    public string name { get; set; }
}



