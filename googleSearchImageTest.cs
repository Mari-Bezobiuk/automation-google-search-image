using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using System.Threading;

namespace googleSearchImageTest
{
    [TestFixture(typeof(ChromeDriver))]
    [TestFixture(typeof(FirefoxDriver))]
    public class BrowserTests<TWebDriver> where TWebDriver : IWebDriver, new()
    {
        private IWebDriver _webDriver;

        private const string _url = "http://google.com";
        private const string _searchRequest = "image";

        private readonly By _searchInput = By.CssSelector("input[type=\"text\"]");
        private readonly By _searchButton = By.XPath("//div[@class=\"FPdoLc lJ9FBc\"]//input[@name='btnK']");
        private readonly By _containsImage = By.XPath("//*[contains(@data-attrid, \"images universal\")][3]");
        private readonly By _currentImage = By.XPath("//*[starts-with(@id, \"dimg\")]");

        [SetUp]
        
        public void SetUp()
        {
            string driversPath = Environment.CurrentDirectory;

            _webDriver = Activator.CreateInstance(typeof(TWebDriver), new object[] { driversPath }) as IWebDriver;

            _webDriver.Navigate().GoToUrl(_url);
            Thread.Sleep(500);

            //Enter the request in the search field
            var searchInput = _webDriver.FindElement(_searchInput);
            searchInput.SendKeys(_searchRequest);
            searchInput.SendKeys(OpenQA.Selenium.Keys.Tab);

            //Click on the search button
            var searchButton = _webDriver.FindElement(_searchButton);
            searchButton.Click();
        }

        [Test]
        public void ImageIsDisplayed()
        {
            //Find the third image on the result page
            try
            {
                var containsImage = _webDriver.FindElement(_containsImage);
                containsImage.Click();

                //Check is this image displayed on the page
                bool imagePresent = containsImage.Displayed;

                Assert.IsTrue(imagePresent, "The image is not exist");
            }
            catch (NoSuchElementException e)
            {
                Console.WriteLine("The image is not exist:");
                Console.WriteLine(e.Message);
            }
        }

        [Test]
        public void ManyImagesDisplayed()
        { 
            try
            {
                //Find all elements with id=dimg - image elements on the page
                var currentImage = _webDriver.FindElements(_currentImage);
                var imageCount = currentImage.Count;
                Assert.NotNull(currentImage, "The page does not contain images.");
                Assert.GreaterOrEqual(imageCount, 12, "The number of images on the page is less than 12.");
            }
            catch (NoSuchElementException e)
            {
                Console.WriteLine("The page does not contain images. ");
                Console.WriteLine(e.Message);
            }
        }

            [TearDown]
        public void TearDown()
        {
            //Close the webdriver window.
            _webDriver.Quit();
        }
    }
}