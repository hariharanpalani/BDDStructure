/*
This is static class file for initializing the web drivers
and page objects.

namespace: WebTestFramework.Common
*/

public static class Pages
{
    private static IWebDriver _webDriver;
    private static readonly InternetExplorerOptions InternetExplorerOptions = new InternetExplorerOptions();
    private static Page _currentPage;
    
    private static readonly LoginPage LoginPage;
    
    static Pages(){
       InitializeWebDriver();
    
       //Initialize all the page objects   
    }
    
    
    private static void InitializeWebDriver()
    {
        var ieDriverService = InternetExplorerDriverService.CreateDefaultService();
        InternetExplorerOptions.UnexpectedAlertBehavior = InternetExplorerUnexpectedAlertBehavior.Accept;
        _webDriver = new InternetExplorerDriver(ieDriverService, InternetExplorerOptions, TimeSpan.FromMinutes(5));
        ieDriverService.LogFile = @"D:\Homeware\IELog.txt";
        ieDriverService.LoggingLevel = InternetExplorerDriverLogLevel.Trace;
        //var firefoxProfile = new FirefoxProfile { AcceptUntrustedCertificates = true, EnableNativeEvents = true };
        //var firefoxBinary = new FirefoxBinary(@"D:\hpalani120109\Stuffs\Software\FP\App\Firefox\firefox.exe");
        //WebDriver = new FirefoxDriver(firefoxBinary, firefoxProfile);
        _webDriver.Manage().Window.Maximize();
        _webDriver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, Settings.DefaultWaitForPage))
            .SetPageLoadTimeout(new TimeSpan(int.MinValue))
            .SetScriptTimeout(new TimeSpan(int.MinValue));
    }
        
           
    public static class Login
    {
        public static LoginPage Page
        {
            get
            {
                _currentPage = LoginPage;
                return LoginPage;
            }
        }
    }
}        
