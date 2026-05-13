using FluentAssertions;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace DatesAndStuff.Web.Tests
{
    [TestFixture]
    public class BlazeDemoTests
    {
        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private string baseURL;
        private bool acceptNextAlert = true;
        
        [SetUp]
        public void SetupTest()
        {
            driver = new FirefoxDriver();
            baseURL = "https://blazedemo.com/";
            verificationErrors = new StringBuilder();
        }
        
        [TearDown]
        public void TeardownTest()
        {
            try
            {
                driver.Quit();
                driver.Dispose();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
            Assert.AreEqual("", verificationErrors.ToString());
        }
        
        [Test]
        public void MexicoDublin_LeastThreeRoutes()
        {
            driver.Navigate().GoToUrl("https://blazedemo.com/");
            new SelectElement(driver.FindElement(By.Name("fromPort"))).SelectByText("Mexico City");
            new SelectElement(driver.FindElement(By.Name("toPort"))).SelectByText("Dublin");
            driver.FindElement(By.XPath("//input[@value='Find Flights']")).Click();
            
            var rows = driver.FindElements(By.XPath("//table/tbody/tr"));

            const double priceThreshold = 210.0;
            var priceCells = driver.FindElements(By.XPath("//table/tbody/tr/td[6]"));
            var values = priceCells
                .Select(c => ParseMoney(c.Text))
                .ToList();
            
            
            if (values.Any(v => v < priceThreshold))
            {
                var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var path = Path.Combine(home, "verval.png");
                
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                screenshot.SaveAsFile(path);
            }

            rows.Count.Should().BeGreaterThanOrEqualTo(3);
        }
        
        private double ParseMoney(string value)
        {
            return double.Parse(
                value.Replace("$", "").Trim(),
                System.Globalization.CultureInfo.InvariantCulture
            );
        }
        
        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        
        private bool IsAlertPresent()
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }
        
        private string CloseAlertAndGetItsText() {
            try {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert) {
                    alert.Accept();
                } else {
                    alert.Dismiss();
                }
                return alertText;
            } finally {
                acceptNextAlert = true;
            }
        }
    }
}