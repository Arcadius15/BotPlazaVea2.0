using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotPlazaVea2._0.Utils
{
    public class Browser
    {
        static string brave = @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe";

        public static async Task<PuppeteerSharp.Browser> getBrowser()
        {
            using var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();

            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = brave,
                Timeout = 300000,
                LogProcess = true
            });

            return browser;
        }
    }
}
