[AttributeUsage(AttributeTargets.Field)]
public class SelectAttributes : Attribute
{
  // Private fields. 
  private int idValue;


  public SelectAttributes(int idValue)
  {
      this.idValue = idValue;

  }

  public virtual int Value
  {
      get { return idValue; }
  }
}
    

public static class EnumHelper
    {
        /// <summary>
        /// Return the enum element based on a string.
        /// </summary>
        /// <remarks>
        /// Unfortunately we are not able to specify <c>where T:Enum</c> (error CS0702)<br />
        /// </remarks>
        /// <typeparam name="T">The Type of enum</typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T StringToEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// Filter the elements of an <see cref="Enum"/> and return the list of strings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filtered">The element to filter.</param>
        /// <returns></returns>
        public static List<string> EnumListFiltered<T>(params T[] filtered)
        {
            var filterList = new List<T>(filtered);

            return new List<string>(
                Enum.GetNames(typeof(T))).FindAll(
                    s => !filterList.Contains(StringToEnum<T>(s)));
        }

        /// <summary>
        /// Enums to string. return name of enum if it has not an [Description] attribute
        /// otherwise it returns [Description] attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anEnum">An enum.</param>
        /// <returns></returns>
        public static string EnumToString<T>(this T anEnum)
        {
            Dictionary<T, string> items = GetDictionaryItemsFromEnum<T>();
            return items[anEnum];
        }

        /// <summary>
        /// Gets the list items from enum.
        /// </summary>
        /// <typeparam name="T">list of filtered enums</typeparam>
        /// <param name="filtered">The filter.</param>
        /// <returns></returns>
        public static Dictionary<T, string> GetDictionaryItemsFromEnum<T>(params T[] filtered)
        {
            var enumType = typeof(T);
            var items = new Dictionary<T, string>();
            var filteredList = EnumListFiltered(filtered);
            foreach (int i in Enum.GetValues(enumType))
            {
                var enumField = enumType.GetField(Enum.GetName(enumType, i));
                var descriptions = enumField.GetCustomAttributes(typeof(DescriptionAttribute), false);
                var name = Enum.GetName(typeof(T), i);
                if (filteredList.Contains(name))
                {
                    items.Add(StringToEnum<T>(name), descriptions.Length == 0 ? name : ((DescriptionAttribute)descriptions[0]).Description);
                }
            }
            return items;
        }

        public static string ListToString<T>(List<T> lists)
        {
            var result = lists.Aggregate("", (current, item) => current + (EnumToString(item) + ", "));
            if (result.Length - 2 > 0)
            { result = result.Substring(0, result.Length - 2); }

            return result;
        }

        public static string EnumToValue<T>(this T anEnum)
        {
            var enumType = typeof(T);
            if (!Enum.IsDefined(enumType, anEnum)) { return string.Empty; }
            var enumValue = (int)Enum.Parse(enumType, (Enum.GetName(enumType, anEnum)));
            return enumValue.ToString(CultureInfo.InvariantCulture);
        }

        public static string EnumToDescription<T>(T @enum)
        {
            Type enumType = typeof(T);
            FieldInfo enumField = enumType.GetField(Enum.GetName(enumType, @enum));
            object[] descriptions = enumField.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return descriptions == null || descriptions.Length == 0 ? @enum.ToString() : ((DescriptionAttribute)descriptions[0]).Description;
        }

        public static T DescriptionToEnum<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) { throw new InvalidOperationException(); }
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                    { return (T)field.GetValue(null); }
                }
                else
                {
                    if (field.Name == description)
                    { return (T)field.GetValue(null); }
                }
            }
            throw new ArgumentException("Not found.", "description");
        }

        /// <summary>
        ///     
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="textField"></param>
        /// <param name="valueField"></param>
        /// <param name="filtered"></param>
        /// <returns></returns>
        public static List<TResult> EnumToType<T, TResult>(string textField, string valueField, params T[] filtered) where TResult : new()
        {
            return EnumToType<T, TResult>(textField, valueField, false, filtered);
        }

 
        
        public static List<TResult> EnumToType<T, TResult>(string textField, string valueField, bool isEmptyRequired = false, params T[] filtered) where TResult : new()
        {
          return GetEnumList<T, TResult>(textField,valueField,isEmptyRequired,-1,filtered);
        }

        
       private  static List<TResult> GetEnumList<T, TResult>(string textField, string valueField, bool isEmptyRequired = false,int SelectAttributeValue=-1, params T[] filtered) where TResult : new()
       {

       
            var enumType = typeof(T);
            var results = new List<TResult>();
            var filteredList = EnumListFiltered(filtered);

            foreach (int value in Enum.GetValues(enumType))
            {
                var name = Enum.GetName(typeof (T), value);

                if(!filteredList.Contains(name))
                    continue;
                
                var enumField = enumType.GetField(Enum.GetName(enumType, value));
                if (SelectAttributeValue > -1)
                {

                    var selectAttributes = ((SelectAttributes[])enumField.GetCustomAttributes(typeof(SelectAttributes), false)).FirstOrDefault();
                    if (selectAttributes != null)
                        if (selectAttributes.Value != SelectAttributeValue)
                            continue;

                }

                var descriptionAttribute = ((DescriptionAttribute[])enumField.GetCustomAttributes(typeof(DescriptionAttribute), false)).FirstOrDefault();
                var text = (descriptionAttribute != null) ? descriptionAttribute.Description : Enum.GetName(typeof(T), value);

                var gType = new TResult();
                var textfieldInfo = gType.GetType().GetProperty(textField, typeof(string));
                var valuefieldInfo = gType.GetType().GetProperty(valueField, typeof(int));

                if (results.Count == 0 && isEmptyRequired)
                {
                    var gTypeTemp = new TResult();
                    textfieldInfo.SetValue(gTypeTemp, string.Empty, null);
                    valuefieldInfo.SetValue(gTypeTemp, -1, null);
                    results.Add(gTypeTemp);
                }
                textfieldInfo.SetValue(gType, text, null);
                valuefieldInfo.SetValue(gType, value, null);
                results.Add(gType);
            }
            return results;
      }

       public static List<TResult> EnumToTypeBasedOnAttribute<T, TResult>(string textField, string valueField, bool isEmptyRequired = false, int SelectAttributeValue = -1, params T[] filtered) where TResult : new()
        {

            return GetEnumList<T,TResult>(textField,valueField,isEmptyRequired,SelectAttributeValue,filtered);

        }
        
        
        
        
        ///// <summary>
        ///// Returns a list of type SelectListItem(Containing enum description or text and value) used  for mvc related bindings
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <returns></returns>
        //public static List<SelectListItem> EnumToList<T>()
        //{
        //    var enumReturn = new List<SelectListItem>();
        //    var enumType = typeof(T);
        //    foreach (int value in Enum.GetValues(enumType))
        //    {
        //        var enumField = enumType.GetField(Enum.GetName(enumType, value));
        //        var descriptionAttribute = ((DescriptionAttribute[])enumField.GetCustomAttributes(typeof(DescriptionAttribute), false)).FirstOrDefault();
        //        var text = (descriptionAttribute != null) ? descriptionAttribute.Description : Enum.GetName(typeof(T), value);
        //        enumReturn.Add(new SelectListItem() { Text = text, Value = value });

        //    }

        //    return enumReturn;
        
        //}
    
    
        public static string EnumToJqGridSelectString<T>(string valuePairCharacter = ":", string splitCharacter = ";", params T[] filtered)
        {
            var items = GetDictionaryItemsFromEnum(filtered);
            string outputString = "";

            foreach (var item in items)
            {
                outputString += string.Format("{0}{1}{2}{3}", item.Key, valuePairCharacter, item.Value, splitCharacter);
            }

            return outputString.TrimEnd(splitCharacter.ToCharArray());
        }
    
    }
