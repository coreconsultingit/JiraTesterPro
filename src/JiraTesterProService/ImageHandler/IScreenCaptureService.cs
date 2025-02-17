﻿using JiraTesterProData.JiraMapper;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraTesterProService.ImageHandler
{
    public interface IScreenCaptureService
    {
        Task<bool> CaptureScreenShot(ScreenShotInputDto inputDto);
        Task<bool> SetStartSession();
        Task CloseSession();

        Task<IPage> GetPage();
    }

}
