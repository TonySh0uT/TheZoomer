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
                Console.WriteLine("Another try");
            }
            catch
            {
                Thread.Sleep(3000);
                loadCheckingWhileJoining(browser);
            }
        }


        public static void loadFirst(IWebDriver browser)
        {
            try
            {
                try
                {
                    IWebElement joinFromBrowserBtn = browser.FindElement(By.LinkText("Join from Your Browser"));
                    joinFromBrowserBtn.Click();
                    Console.WriteLine("He found it 2");
                }
                catch
                {
                    IWebElement joinFromBrowserBtn = browser.FindElement(By.LinkText("Войдите с помощью браузера"));
                    joinFromBrowserBtn.Click();
                    Console.WriteLine("He found it 3");
                }
            }
            catch
            {
                Thread.Sleep(3000);
                loadFirst(browser);
                Console.WriteLine("Another try");
            }
        }
        
        
        
        public static void Main(string[] args)
        {



            for (int i = 0; i < 3; i++)
            {
                //string username = args[0].ToString();
                string username = "Cock ";
                var firefoxOptions = new OpenQA.Selenium.Firefox.FirefoxOptions();
                firefoxOptions.AddArguments("--headless");
                var firefoxDriverService = FirefoxDriverService.CreateDefaultService();
                IWebDriver browser = new FirefoxDriver(firefoxDriverService, firefoxOptions);
                browser.Navigate()
                    .GoToUrl("https://us04web.zoom.us/j/75496093147?pwd=VUxvb2NOeVdNdUZFSGJ1VEJhTEdVUT09");
                loadCheckingWhileJoining(browser);
                try
                {
                    IWebElement joinFromBrowser = browser.FindElement(By.LinkText("Join from Your Browser"));
                }
                catch
                {
                    IWebElement launch = browser.FindElement(By.CssSelector("div[role='button']"));
                    Console.WriteLine("He found it");
                    launch.Click();
                    Thread.Sleep(2000);
                }
                loadFirst(browser);

                IWebElement name = browser.FindElement(By.Name("inputname"));
                name.SendKeys($"{username + i.ToString()}");
                IWebElement joinBtn = browser.FindElement(By.ClassName("submit"));
                joinBtn.Click();
                Thread.Sleep(5000);
            }

        }
    }
}