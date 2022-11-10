using JiraTesterProData;
using PuppeteerSharp;

namespace JiraTesterProService.ImageHandler;

public class ScreenCaptureService : IScreenCaptureService
{
    private IUserCredentialProvider userCredentialProvider;

    private ILogger<ScreenCaptureService> logger;
    private bool loginsucessfull = false;
    private IBrowser browser;
    private IPage page;
    public ScreenCaptureService(ILogger<ScreenCaptureService> logger, IUserCredentialProvider userCredentialProvider)
    {
        this.logger = logger;
        this.userCredentialProvider = userCredentialProvider;
    }
    
    public async Task<bool> SetStartSession()
    {
        await DownloadBrowserAsync();

        browser = await Puppeteer.LaunchAsync(new LaunchOptions()
        {
            Headless = false,
            DefaultViewport = new ViewPortOptions()
            {
                Width = 1920,
                Height = 1080
            }
        });

        var userCredential = userCredentialProvider.GetJiraCredential();
        page = await browser.NewPageAsync();
        if (!loginsucessfull && userCredential != null)
        {
            try
            {
                await page.GoToAsync(userCredential.LoginUrl);
                await page.TypeAsync("#login-form-username", userCredential.UserName);
                await page.TypeAsync("#login-form-password", userCredential.Password);
                await page.ClickAsync("#login");
                await page.WaitForNavigationAsync();
            }
            catch (Exception e)
            {
               logger.LogError(e.Message);
               return false;
            }
          

            loginsucessfull = true;
        }


        return loginsucessfull;
    }

    public async Task<bool> CaptureScreenShot(ScreenShotInputDto inputDto)
    {
        try
        {
            var screenshot =  await ScreenshotUrlAsync(inputDto);
            var dirInfo = new FileInfo(inputDto.FilePath).Directory;
            if (!dirInfo.Exists)
            {
                Directory.CreateDirectory(dirInfo.FullName);
            }

            await File.WriteAllBytesAsync(inputDto.FilePath, screenshot);
            
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return false;
        }
        return true;
    }

    private async Task<byte[]> ScreenshotUrlAsync(ScreenShotInputDto inputDto
        )
    {

        // First download the browser (this will only happen once)
        
        // Create a new tab/page in the browser and navigate to the URL
        
        try
        {
            await page.GoToAsync(inputDto.TestUrl);
           
            // Screenshot the page and return the byte stream
            var bytes = await page.ScreenshotDataAsync();
            return bytes;
        }
        catch (Exception e)
        {
           logger.LogError(e.Message);
            throw;
        }
        finally
        {
           

        }

        
    }

    public async Task CloseBrowserAndPage()
    {
        if (page != null)
        {
            await page.CloseAsync();
        }

        if (browser != null)
        {
            await browser.CloseAsync();
        }
       
        
    }

    private async Task DownloadBrowserAsync()
    {
        using var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
    }
}