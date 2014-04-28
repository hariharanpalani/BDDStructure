/*
  The base file for 

*/
public class Page{
    public IWebDriver Driver { get; set; }

    public string ExpectedTitle { get; set; }
  
    public Page(IWebDriver webDriver)
    {
      Driver = webDriver;
    }
    
    public virtual void Open()
    {
        //Any common code for all the pages.
    }
    
    public virtual bool IsDisplayed()
    {
        return Driver.Title.Equals(ExpectedTitle);
    }
    
    public bool Exists(string id)
    {
        Driver.SetDefaultImplicitTimeOut();
        Driver.Focus(id);
        if (Driver.IsElementPresent(By.Id(id)))
        {
            Driver.RestoreImplicitTimeOut();
            return true;
        }
        Driver.RestoreImplicitTimeOut();
        return false;
    }
    
    public virtual Page Initialize()
    {
        new WebDriverWait(Driver, new TimeSpan(0, 3, 10)).Until(ExpectedConditions.ElementExists(By.TagName("body")));
        PageFactory.InitElements(Driver, this);
        return this;
    }
    
    
}
