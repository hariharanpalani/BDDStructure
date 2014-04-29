using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;


public static class ExtensionMethods
{
    public static PropertyInfo GetProperty<TAttribute>(this object instance, bool inherit = false) where TAttribute : class
    {
        if (instance == null) throw new ArgumentNullException("instance");
        var attributeProperty = (from property in instance.GetType().GetProperties()
                                 let propertyAttribute = property.GetCustomAttributes(typeof(TAttribute), false)
                                 where propertyAttribute != null && propertyAttribute.Length > 0
                                 select property).FirstOrDefault();

        return attributeProperty;
    }
  
    
    public static TResult GetValue<T, TResult>(this object instance) where T : class
    {
        var property = instance.GetProperty<T>();
        return (TResult)property.GetValue(instance, null);
    }
}
