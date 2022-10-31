using CommandLine.Text;
using CommandLine;

namespace JiraTesterProData
{
    public class JiraTesterCommandLineOptions
    {
        [Option('i', "inputjiratestfile", Required = false, HelpText = "Your Jira test input file")]
        public string? InputJiraTestFile { get; set; }

        [Option('g', "generatejiratestfile", Required = false, HelpText = "Your Jira bug matrix")]
        public bool? GenerateJiraTestInput { get; set; }


        [Option('j', "jiraurl", Required = false, HelpText = "Your jira url")]
        public string? JiraUrl { get; set; }

        [Option('o', "outputjiratestfile", Required = false, HelpText = "Your Jira test result file")]
        public string? OutputJiraTestFile { get; set; }

        [Option('u', "username", Required = false, HelpText = "Your username")]
        public string? Username { get; set; }
        [Option('p', "password", Required = false, HelpText = "Your password")]
        public string? Password { get; set; }

        public override string ToString()
        {
            return $"Running with username {Username} password {Password} JiraUrl {JiraUrl}  Jira input file {InputJiraTestFile}";
        }
    }
}