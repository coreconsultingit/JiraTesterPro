using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace JiraTesterProData
{
    public abstract class JiraTestStrategy
    {
        public abstract Task<JiraTestResult> Execute(JiraTestMasterDto jiraTestMasterDto);
    }
}
