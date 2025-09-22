using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;

namespace Prerendering
{
    public class PrerenderException : Exception
    {
        public PrerenderException(string message, Exception innerException) : base(message, innerException)
        {

        }
        public PrerenderException(string message) : base(message)
        {

        }
    }
}