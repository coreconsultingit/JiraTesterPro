

namespace JiraTesterProService;

public class JiraClientProvider : IJiraClientProvider
{
   

    private IUserCredentialProvider userCredentialProvider;
    public JiraClientProvider(IUserCredentialProvider userCredentialProvider)
    {
        
        this.userCredentialProvider = userCredentialProvider;
    }

    public Jira GetJiraClient(JiraRestClientSettings? settings)
    {
        var credential = userCredentialProvider.GetJiraCredential();
       
        if (settings == null)
        {
            settings = new JiraRestClientSettings()
            {
                EnableRequestTrace = true

            };
        }
        
        settings.CustomFieldSerializers["com.atlassian.jira.plugin.system.customfieldtypes:multiuserpicker"]
            = new MultiObjectCustomFieldValueSerializer("displayName");
        return Jira.CreateRestClient(

            credential.LoginUrl, credential.UserName, credential.Password, settings);
    }

   
}