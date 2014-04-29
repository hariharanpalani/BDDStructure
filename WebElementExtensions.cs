using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace WebTestFramework.Common
{
    public static class WebElementExtensions
    {
        public static void ClearAndFill(this IWebElement element, string text)
        {
            if (text == null)
            {
                return;
            }
            element.Clear();
            element.SendKeys(text);
        }

        public static void ClearAndFill(this IWebElement element, DateTime? dateTime)
        {
            element.Clear();
            if (dateTime.HasValue)
            {
                element.SendKeys(dateTime.Value.ToShortDateString());
            }
        }

        public static void Fill(this IWebElement element, string text)
        {
            if (text == null)
            {
                return;
            }
            element.SendKeys(text);
        }

        public static void Fill(this IWebElement element, DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return;
            }

            element.SendKeys(dateTime.Value.ToShortDateString());
        }

        public static bool IsElementPresent(this IWebElement element)
        {
            try
            {
                var e = element.Displayed;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsElementPresent(this ISearchContext context, By findBy)
        {
            try
            {
                return context.FindElement(findBy).Displayed;
                //return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetValue(this IWebElement element)
        {
            string readOnly = element.GetAttribute("readonly");
            return readOnly == "true" ? element.GetAttribute("Value") : element.Text;
        }

        public static IList<TResult> ForEach<TResult>(this IWebDriver driver, By findBy, Func<IWebElement, TResult> func)
        {
            var elements = driver.FindElements(findBy);

            if (elements == null || elements.Count == 0)
            {
                return null;
            }

            return elements.Select(func).ToList();
        }

        public static void SelectByValue(this IWebElement element, string value)
        {
            var selectElement = new SelectElement(element);
            selectElement.SelectByValue(value);
        }

        public static void SelectByText(this IWebElement element, string value)
        {
            var selectElement = new SelectElement(element);
            selectElement.SelectByText(value);
        }

        public static void SetText(this IWebDriver driver, string id, string value)
        {
            if (value == null)
            {
                return;
            }
            var jsExecutor = driver as IJavaScriptExecutor;
            if (jsExecutor != null)
            {
                jsExecutor.ExecuteScript(string.Format("$('#{0}').val('{1}');", id, value));
            }
        }

        public static void FocusAndClick(this IWebDriver driver, string id)
        {
            var jsExecutor = driver as IJavaScriptExecutor;
            if (jsExecutor != null)
            {
                jsExecutor.ExecuteScript(string.Format("$('#{0}').focus();", id));
            }
            driver.FindElement(By.Id(id)).Click();
        }

        public static void AjaxClick(this IWebDriver driver, string id)
        {
            FocusAndClick(driver, id);
            driver.AjaxWait();
        }

        public static void WaitAndClick(this IWebDriver driver, int milliseconds, string id)
        {
            driver.DoWait(milliseconds);
            var jsExecutor = driver as IJavaScriptExecutor;
            if (jsExecutor != null)
            {
                jsExecutor.ExecuteScript(string.Format("$('#{0}').click();", id));
            }
        }

        public static void DoWait(this IWebDriver driver, int milliseconds)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(milliseconds));
            wait.Until(
                            arg =>
                            {
                                Thread.Sleep(milliseconds);
                                return true;
                            });
        }

        public static void ScrollToElement(this IWebDriver driver, string parentId, string elementId)
        {
            var jsExecutor = driver as IJavaScriptExecutor;
            if (jsExecutor != null)
            {
                jsExecutor.ExecuteScript(string.Format("$('#{0}').scrollTo($('#{1}'));", parentId, elementId));
            }
        }

        public static void Focus(this IWebDriver driver, string id)
        {
            var jsExecutor = driver as IJavaScriptExecutor;
            if (jsExecutor != null)
            {
                jsExecutor.ExecuteScript(string.Format("$('#{0}').focus();", id));
            }
        }

        public static void SetText(this IWebDriver driver, string id, DateTime? value)
        {
            if (!value.HasValue)
            {
                return;
            }
            var jsExecutor = driver as IJavaScriptExecutor;
            if (jsExecutor != null)
            {
                jsExecutor.ExecuteScript(string.Format("$('#{0}').val('{1}');", id, value.Value.ToShortDateString()));
            }
        }

        public static void SetTextAndWait(this IWebDriver driver, int milliSeconds, Func<bool> condition, string id, string value, string errorDescription)
        {
            if (value == null)
            {
                return;
            }
            var jsExecutor = driver as IJavaScriptExecutor;
            if (jsExecutor != null)
            {
                jsExecutor.ExecuteScript(string.Format("$('#{0}').val('{1}');", id, value));
            }
            var timeElapsed = 0;
            while (!condition() && timeElapsed < milliSeconds)
            {
                Thread.Sleep(100);
                timeElapsed += 100;
            }

            if (timeElapsed >= milliSeconds || !condition())
            {
                throw new TimeoutException("Timed out while waiting for: " + errorDescription);
            }
        }

        public static void ClickAndWait(this IWebElement webElement, int milliSeconds, Func<bool> condition, string errorDescription)
        {
            webElement.Click();
            WaitHandler(condition, milliSeconds, errorDescription);
            /*var timeElapsed = 0;
            while (!condition() && timeElapsed < milliSeconds)
            {
                Thread.Sleep(100);
                timeElapsed += 100;
            }

            if (timeElapsed >= milliSeconds || !condition())
            {
                throw new TimeoutException("Timed out while waiting for: " + errorDescription);
            }*/
        }

        public static void WaitUntil(this IWebDriver driver, By by, int milliSeconds)
        {
            new WebDriverWait(driver, new TimeSpan(0, 0, milliSeconds)).Until(ExpectedConditions.ElementIsVisible(by));
        }

        public static void WaitUntil(this IWebDriver driver, Func<bool> condition, int milliSeconds, string errorDescription)
        {
            WaitHandler(condition, milliSeconds, errorDescription);
        }

        public static bool WaitTilljQueryInitialize(this IWebDriver driver)
        {
            var ajaxComplete = false;
            while (true)
            {
                var javaScriptExecutor = driver as IJavaScriptExecutor;
                if (javaScriptExecutor == null) { break; }
                ajaxComplete = (bool)javaScriptExecutor.ExecuteScript("return typeof jQuery != undefined");
                if (ajaxComplete)
                {
                    break;
                }
                Thread.Sleep(100);
            }
            return ajaxComplete;
        }

        public static void AjaxWait(this IWebDriver driver)
        {
            try
            {
                while (true)
                {
                    var javaScriptExecutor = driver as IJavaScriptExecutor;
                    if (javaScriptExecutor == null) { break; }
                    var ajaxComplete = (bool)javaScriptExecutor.ExecuteScript("return jQuery.active == 0");
                    if (ajaxComplete)
                    {
                        break;
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }

        private static void WaitHandler(Func<bool> condition, int milliSeconds, string errorDescription)
        {
            var timeElapsed = 0;
            while (!condition() && timeElapsed < milliSeconds)
            {
                Thread.Sleep(100);
                timeElapsed += 100;
            }

            if (timeElapsed >= milliSeconds || !condition())
            {
                throw new TimeoutException("Timed out while waiting for: " + errorDescription);
            }
        }

        public static void MenuClick(this IWebDriver driver, string parentId, string childId)
        {
            var builder = new Actions(driver);

            if (driver.IsElementPresent(By.Id(parentId)))
            {
                var parentElement = driver.FindElement(By.Id(parentId));
                builder.MoveToElement(parentElement).Perform();
                var javaScriptExecutor = driver as IJavaScriptExecutor;
                if (javaScriptExecutor == null)
                {
                    return;
                }

                javaScriptExecutor.ExecuteScript("document.getElementById('" + childId + "').click();");
            }
        }

        public static void MenuClick(this IWebDriver driver, string id)
        {
            var javaScriptExecutor = driver as IJavaScriptExecutor;
            if (javaScriptExecutor == null)
            {
                return;
            }
            javaScriptExecutor.ExecuteScript("document.getElementById('" + id + "').click();");
        }

        public static void Select(this IWebElement element)
        {
            var visible = element.IsElementPresent();

            if (!visible)
            {
                return;
            }

            if (!element.Selected)
            {
                element.Click();
            }
        }

        public static void UnSelect(this IWebElement element)
        {
            var visible = element.IsElementPresent();

            if (!visible) { return; }

            if (element.Selected)
            {
                element.Click();
            }
        }

        public static IWebElement ScrollTo(this IWebDriver driver, By by)
        {
            var element = (RemoteWebElement) driver.FindElement(by);
            var hack = element.LocationOnScreenOnceScrolledIntoView;
            return element;
        }

        public static void AjaxWaitForElementPresent(this IWebDriver driver, By by, int timeOutInSeconds)
        {
            try
            {
                //nullify the implicit wait timeout for the driver
                driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, 0));

                //wait for control to visible 
                driver.WaitUntil(() => driver.IsElementPresent(@by), timeOutInSeconds, "Ajax request failed during wait...");

                //reset the implicit wait timeout to default value
                driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, Settings.DefaultWaitForPage));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static void AjaxWaitForElementNotPresent(this IWebDriver driver, By by, int timeOutInSeconds)
        {
            try
            {
                //nullify the implicit wait timeout for the driver
                driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, 0));

                //wait for control to visible 
                driver.WaitUntil(() => !driver.IsElementPresent(@by), timeOutInSeconds, "Ajax request failed during wait...");

                //reset the implicit wait timeout to default value
                driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, Settings.DefaultWaitForPage));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void WaitForMenu(this IWebDriver driver, By parentBy, By childBy, int timeOutInSeconds)
        {
            try
            {
                var builder = new Actions(driver);
                if (driver.IsElementPresent(parentBy))
                {
                    var parentElement = driver.FindElement(parentBy);
                    builder.MoveToElement(parentElement).Perform();
                    driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, 0));
                    driver.WaitUntil(() => driver.IsElementPresent(childBy), timeOutInSeconds, "Menu is not found...");
                    driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, Settings.DefaultWaitForPage));
                }
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        public static void SetDefaultImplicitTimeOut(this IWebDriver driver)
        {
            driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, 60));
        }

        public static void RestoreImplicitTimeOut(this IWebDriver driver)
        {
            driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, Settings.DefaultWaitForPage));
        }
    }
}
