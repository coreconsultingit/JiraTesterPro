using CommandLine.Text;
using CommandLine;

namespace JiraTesterProData
{
    public class JiraTesterCommandLineOptions
    {
        [Option('j', "jiraurl", Required = true, HelpText = "Your jira url")]
        public string JiraUrl { get; set; }

        [Option('u', "username", Required = true, HelpText = "Your username")]
        public string Username { get; set; }
        [Option('p', "password", Required = true, HelpText = "Your password")]
        public string Password { get; set; }

        public override string ToString()
        {
            return $"Running with username {Username} password {Password} JiraUrl {JiraUrl}";
        }
    }
}