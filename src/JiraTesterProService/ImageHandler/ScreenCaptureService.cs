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

    public async Task<bool> CaptureScreenShot(ScreenShotInputDto inputDto)
    {
        try
        {
            var screenshot =  ScreenshotUrlAsync(inputDto).Result;
            await File.WriteAllBytesAsync(inputDto.FilePath, screenshot);
            
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return false;
        }
        return true;
    }

    private async Task<byte[]> ScreenshotUrlAsync(ScreenShotInputDto inputDto)
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

            if (!loginsucessfull  && ! string.IsNullOrEmpty(inputDto.LoginUrl))
            {
                await page.GoToAsync(inputDto.LoginUrl);
                await page.TypeAsync("#login-form-username", inputDto.UserName);
                await page.TypeAsync("#login-form-password", inputDto.Password);
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