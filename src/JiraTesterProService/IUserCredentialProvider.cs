

namespace JiraTesterProService
{
    public interface IUserCredentialProvider
    {
        JiraLogInDto GetJiraCredential();

        void AddJiraCredential(JiraLogInDto loginDto);
    }
}
