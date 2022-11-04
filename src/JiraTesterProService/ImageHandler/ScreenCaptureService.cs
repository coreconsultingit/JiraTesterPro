using PuppeteerSharp;

namespace JiraTesterProService.ImageHandler;

public class ScreenCaptureService : IScreenCaptureService
{

    private ILogger<ScreenCaptureService> logger;
    private bool loginsucessfull = false;
    public ScreenCaptureService(ILogger<ScreenCaptureService> logger)
    {
        this.logger = logger;
    }

    public async Task<bool> CaptureScreenShot(ScreenShotInputDto inputDto, ScreenShotLogInScreenDto loginDto)
    {
        try
        {
            var screenshot =  await ScreenshotUrlAsync(inputDto, loginDto);
            await File.WriteAllBytesAsync(inputDto.FilePath, screenshot);
            
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return false;
        }
        return true;
    }

    private async Task<byte[]> ScreenshotUrlAsync(ScreenShotInputDto inputDto,
        ScreenShotLogInScreenDto screenShotLogInScreenDto)
    {

        // First download the browser (this will only happen once)
        await DownloadBrowserAsync();
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions()
        {
            Headless = true,
            DefaultViewport = new ViewPortOptions()
            {
                Width = 1920,
                Height = 1080
            }
        });
        // Create a new tab/page in the browser and navigate to the URL
        var page = await browser.NewPageAsync();
        try
        {

            if (!loginsucessfull  && screenShotLogInScreenDto!=null)
            {
                await page.GoToAsync(screenShotLogInScreenDto.LoginUrl);
                await page.TypeAsync("#login-form-username", screenShotLogInScreenDto.UserName);
                await page.TypeAsync("#login-form-password", screenShotLogInScreenDto.Password);
                await page.ClickAsync("#login");
                await page.WaitForNavigationAsync();

                loginsucessfull = true;
            }

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
            await page.CloseAsync();
            await browser.CloseAsync();

        }

        
    }

    private async Task DownloadBrowserAsync()
    {
        using var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
    }
}