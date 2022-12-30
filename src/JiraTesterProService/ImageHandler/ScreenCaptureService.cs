using System.Diagnostics;
using System.Drawing.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Atlassian.Jira;
using JiraTesterProData;
using JiraTesterProData.JiraMapper;
using JiraTesterProService.JiraHtmlHelper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Finance.Implementations;
using PuppeteerSharp;
using PuppeteerSharp.Input;

namespace JiraTesterProService.ImageHandler;

public class ScreenCaptureService : IScreenCaptureService
{
    private IUserCredentialProvider userCredentialProvider;
    private IRetryHelper retryHelper;
    private ILogger<ScreenCaptureService> logger;
    private bool loginsucessfull = false;
    private IBrowser browser;
    private IPage page;
    private IConfiguration configuration;
    private IJiraClientProvider clientProvider;


    public ScreenCaptureService(ILogger<ScreenCaptureService> logger, IUserCredentialProvider userCredentialProvider, IConfiguration configuration, IJiraClientProvider clientProvider, IRetryHelper retryHelper)
    {
        this.logger = logger;
        this.userCredentialProvider = userCredentialProvider;
        this.configuration = configuration;
        this.clientProvider = clientProvider;
        this.retryHelper = retryHelper;
    }

    private async Task<bool> StartLoginSession()
    {
        try
        {

            //var path = Microsoft.Win32.Registry.GetValue(
            //    @"HKEY_CLASSES_ROOT\ChromeHTML\shell\open\command", null, null) as string;
            //string path = null;
            //if (path != null)
            //{
            //    var split = path.Split('\"');
            //    path = split.Length >= 2 ? split[1] : null;
            //}
            //else
            //{
                await DownloadBrowserAsync();
            //}
            var dumpIO = configuration.GetValue<bool?>("DumpIO") ?? false;

            var args = new List<string>()
            {
                "--no-sandbox", "--start-maximized"
            };
            var launchoptions = new LaunchOptions()
            {
                Headless = configuration.GetValue<bool>("EnableHeadlessMode"),
                DefaultViewport = new ViewPortOptions()
                {
                    Width = 0,
                    Height = 0
                },
                IgnoreDefaultArgs = false,
                Args = args.ToArray(),
                DumpIO= dumpIO
            };

            //if (path != null)
            //{
            //    launchoptions.ExecutablePath = path;
            //}
            browser = await Puppeteer.LaunchAsync(launchoptions);


            var userCredential = userCredentialProvider.GetJiraCredential();
            page = (await browser.PagesAsync())[0];

            if (!loginsucessfull && userCredential != null)
            {
                try
                {
                    await page.GoToAsync(userCredential.LoginUrl);
                    await page.WaitForSelectorAsync("#login-form-username");
                    await page.TypeAsync("#login-form-username", userCredential.UserName);
                    await page.TypeAsync("#login-form-password", userCredential.Password);
                    var rememberhandler = await page.QuerySelectorAsync("#login-form-remember-me");
                    await rememberhandler.EvaluateFunctionAsync("b=>b.click()");

                    await page.ClickAsync("#login");
                   // await page.WaitForNavigationAsync();
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                    throw;
                }


                loginsucessfull = true;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            throw;
        }



        return loginsucessfull;
    }
    
    public async Task<bool> SetStartSession()
    {
        return  await  retryHelper.Retry(async () => await StartLoginSession(), 2);
    }

    public async Task<bool> CaptureScreenShot(ScreenShotInputDto inputDto)
    {
        try
        {
            if (!loginsucessfull)
            {
                loginsucessfull = await SetStartSession();
                if (!loginsucessfull)
                {
                    throw new Exception("Unable to login");
                }
            }


            byte[] screenshot = inputDto.ScreenShot;
            if (screenshot == null)
            {
                screenshot = await ScreenshotUrlAsync(inputDto);
            }
            
            var dirInfo = new FileInfo(inputDto.FilePath).Directory;
            if (!dirInfo.Exists)
            {
                Directory.CreateDirectory(dirInfo.FullName);
            }

            if (screenshot != null)
            {
                await File.WriteAllBytesAsync(inputDto.FilePath, screenshot);
            }
            else
            {
                logger.LogError($"No screenshot details available for {inputDto.FilePath}");
            }
           
            
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
        var screenshotpage = await browser.NewPageAsync();
        try
        {
           
            await screenshotpage.GoToAsync(inputDto.TestUrl);
           
            // Screenshot the page and return the byte stream
            var bytes = await screenshotpage.ScreenshotDataAsync(new ScreenshotOptions()
            {
                FullPage = true
            });
            return bytes;
        }
        catch (Exception e)
        {
           logger.LogError(e.Message);
           return null;
        }
        finally
        {
            if (screenshotpage != null)
            {
                await screenshotpage.CloseAsync();
            }
            

        }

        
    }

    public async Task CloseSession()
    {
        try
        {
            if (page != null)
            {
                await page.CloseAsync();
            }
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }
        finally
        {
            if (browser != null)
            {
                await browser.CloseAsync();
            }
        }
    }

    
    
    public async Task<IPage> GetPage()
    {
        var page = await browser.NewPageAsync();
        page.DefaultNavigationTimeout = 50000;
        return page;
    }
    

    private async Task DownloadBrowserAsync()
    {
        using var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
    }

   
}