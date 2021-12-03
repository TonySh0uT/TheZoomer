using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;

namespace ZoomPart
{
    public class Program
    {

        public static void loadCheckingWhileJoining(IWebDriver browser)
        {
            try
            {
                IWebElement launch = browser.FindElement(By.CssSelector("div[role='button']"));
            }
            catch
            {
                Thread.Sleep(3000);
                loadCheckingWhileJoining(browser);
            }
        }
        
        
        
        
        
        public static void Main(string[] args)
        {
            var firefoxOptions = new OpenQA.Selenium.Firefox.FirefoxOptions();
            //firefoxOptions.AddArguments("--headless");
            var firefoxDriverService = FirefoxDriverService.CreateDefaultService();
            IWebDriver browser = new FirefoxDriver(firefoxDriverService, firefoxOptions);
            browser.Navigate().GoToUrl("https://us04web.zoom.us/j/72064796115?pwd=RnV0YzFsT1NGMlhJWGpxZlY0MjRuQT09");
            loadCheckingWhileJoining(browser);
            try
            {
                IWebElement joinFromBrowser = browser.FindElement(By.LinkText("Войдите с помощью браузера"));
            }
            catch
            {
                IWebElement launch = browser.FindElement(By.CssSelector("div[role='button']"));
                launch.Click();
            }
           
            IWebElement joinFromBrowserBtn = browser.FindElement(By.LinkText("Войдите с помощью браузера"));
            joinFromBrowserBtn.Click();
            IWebElement name = browser.FindElement(By.Name("inputname"));
            name.SendKeys("Игорь");
            IWebElement joinBtn = browser.FindElement(By.ClassName("submit"));
            joinBtn.Click();
            
            
            
            Thread.Sleep(15000);
            
            
            
        }
    }
}