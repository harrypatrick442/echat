using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;

namespace Prerendering
{
    public static class PrerenderingHelper
    {
        static PrerenderingHelper()
        {
            InstallChromeDriver();
        }
        public static string Prerender(string url, Action<IWebDriver> manipulatePage = null)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
#if !DEBUG
            chromeOptions.AddArgument("headless");
#endif
            using (IWebDriver webDriver = new ChromeDriver(chromeOptions))
            {
                try
                {
                    webDriver.Navigate().GoToUrl(url);
                    manipulatePage?.Invoke(webDriver);
                    string html = webDriver.PageSource;
                    return html;
                }
                finally
                {
                    webDriver.Quit();
                }
            }
        }
        private static void InstallChromeDriver()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
        }
    }
}