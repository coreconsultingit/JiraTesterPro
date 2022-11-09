namespace JiraTesterProService;

public class UserCredentialProvider : IUserCredentialProvider
{
    private IMemoryCache memoryCache;
    
    private ILogger<UserCredentialProvider> logger;
    public UserCredentialProvider(IMemoryCache memoryCache, ILogger<UserCredentialProvider> logger)
    {
        this.memoryCache = memoryCache;
       
        this.logger = logger;
    }

    public JiraLogInDto GetJiraCredential()
    {

        memoryCache.TryGetValue(CacheConst.UserLogin, out JiraLogInDto loginDto);

        if (loginDto == null)
        {
            logger.LogError("User cache not initialized");
            throw new Exception("User cache not initialized");
        }

        return loginDto;
    }

    public void AddJiraCredential(JiraLogInDto loginDto)
    {
        memoryCache.Set(CacheConst.UserLogin, loginDto);
    }
}