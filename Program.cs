using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using NUnit.Framework;

namespace CloudQAAutomationTests
{
    [TestFixture]
    public class AutomationPracticeFormTests
    {
        private IWebDriver driver = null!;
        private WebDriverWait wait = null!;
        private const string BASE_URL = "http://app.cloudqa.io/home/AutomationPracticeForm";

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            driver.Navigate().GoToUrl(BASE_URL);
        }

        // Resilient element finder with multiple locator strategies
        private IWebElement FindElementWithFallback(params By[] locators)
        {
            foreach (var locator in locators)
            {
                try
                {
                    var element = wait.Until(d => d.FindElement(locator));
                    if (element.Displayed && element.Enabled)
                        return element;
                }
                catch (NoSuchElementException) { continue; }
                catch (WebDriverTimeoutException) { continue; }
            }
            throw new NoSuchElementException("Element not found with any provided locator");
        }

        [Test]
        public void TestFirstNameField()
        {
            // Multiple locator strategies for First Name field
            var firstNameField = FindElementWithFallback(
                By.Id("fname"),
                By.Name("firstname"),
                By.CssSelector("input[placeholder*='First']"),
                By.XPath("//input[contains(@placeholder, 'First') or contains(@name, 'first')]"),
                By.CssSelector("form input[type='text']:first-of-type")
            );

            // Test the field
            string testData = "John";
            firstNameField.Clear();
            firstNameField.SendKeys(testData);
            
            Assert.That(firstNameField.GetAttribute("value"), Is.EqualTo(testData), 
                "First name field value should match input");
        }

        [Test]
        public void TestEmailField()
        {
            // Multiple locator strategies for Email field
            var emailField = FindElementWithFallback(
                By.Id("email"),
                By.Name("email"),
                By.CssSelector("input[type='email']"),
                By.CssSelector("input[type='text'][placeholder*='mail' i]"),
                By.XPath("//input[contains(@placeholder, 'mail') or contains(@name, 'mail')]")
            );

            // Test the field with valid email
            string validEmail = "test@example.com";
            emailField.Clear();
            emailField.SendKeys(validEmail);
            
            Assert.That(emailField.GetAttribute("value"), Is.EqualTo(validEmail), 
                "Email field should accept valid email");
            
            // Verify field accepts email format
            Assert.That(emailField.GetAttribute("value"), Does.Contain("@"), 
                "Email field should contain @ symbol");
        }

        [Test]
        public void TestGenderRadioButton()
        {
            // Multiple locator strategies for Gender radio button (Male)
            var maleRadioButton = FindElementWithFallback(
                By.Id("male"),
                By.CssSelector("input[value='Male'][type='radio']"),
                By.CssSelector("input[value='male'][type='radio']"),
                By.XPath("//input[@type='radio' and (@value='male' or @value='Male' or @id='male')]"),
                By.XPath("//label[contains(text(), 'Male')]//preceding-sibling::input[@type='radio'] | //label[contains(text(), 'Male')]//following-sibling::input[@type='radio']")
            );

            // Test the radio button
            if (!maleRadioButton.Selected)
            {
                maleRadioButton.Click();
            }
            
            Assert.That(maleRadioButton.Selected, Is.True, 
                "Male radio button should be selected after click");
            
            // Verify the radio button value (case-insensitive check)
            string actualValue = maleRadioButton.GetAttribute("value");
            Assert.That(actualValue.ToLower(), Is.EqualTo("male"), 
                "Selected radio button value should be 'male' (case-insensitive)");
        }

        [TearDown]
        public void Cleanup()
        {
            driver?.Quit();
            driver?.Dispose();
        }
    }
}
