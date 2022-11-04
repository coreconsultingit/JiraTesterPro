namespace JiraTesterProData;
/*
public class Rootobject
{
    public string name { get; set; }
    public string description { get; set; }
    public Status[] statuses { get; set; }
    public Transition[] transitions { get; set; }
}

public class Status
{
    public string id { get; set; }
    public Properties properties { get; set; }
}

public class Properties
{
    public string jiraissueeditable { get; set; }
}

public class Transition
{
    public string name { get; set; }
    public string[] from { get; set; }
    public string to { get; set; }
    public string type { get; set; }
    public Screen screen { get; set; }
    public Rules rules { get; set; }
    public Properties1 properties { get; set; }
}

public class Screen
{
    public string id { get; set; }
}

public class Rules
{
    public Postfunction[] postFunctions { get; set; }
    public Conditions conditions { get; set; }
    public Validator[] validators { get; set; }
}

public class Conditions
{
    public Condition[] conditions { get; set; }
    public string _operator { get; set; }
}

public class Condition
{
    public string type { get; set; }
    public Configuration configuration { get; set; }
}

public class Postfunction
{
    public string type { get; set; }
    public Configuration1 configuration { get; set; }
}

public class Configuration1
{
    public string fieldId { get; set; }
}

public class Validator
{
    public Configuration2 configuration { get; set; }
    public string type { get; set; }
}

public class Configuration2
{
    public Parentstatus[] parentStatuses { get; set; }
    public string permissionKey { get; set; }
}

public class Parentstatus
{
    public string id { get; set; }
}

public class Properties1
{
    public string customproperty { get; set; }
}
*/
public class JiraTestMasterDto
{
    public int StepId { get; set; }

    public string ParentIssueKey{ get; set; }

    public string Scenario { get; set; }

    public string IssueKey { get; set; }

    public string GroupKey { get; set; }
    public int OrderId { get; set; }

    public string Project { get; set; }

    public string IssueType { get; set; }

    public string Component { get; set; }

    public string Status { get; set; }

    public string CustomFieldInput { get; set; }
    public string Summary { get; set; }

    public string Action { get; set; }

    public string Expectation { get; set; }

    public string ExpectedStatus { get; set; }

    public int? ExpectedSubTaskCount { get; set; }
    public string ExpectedSubTaskList { get; set; }

    public bool IsSubTask { get; set; }
    
}