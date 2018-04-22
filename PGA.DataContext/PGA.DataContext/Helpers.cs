namespace PGA.DataContext
{
    public static class ExtensionMethods
    {
        public static string UppercaseFirstLetter(this string value)
        {
            //
            // Uppercase the first letter in the string.
            //
            if (value.Length > 0)
            {
                var array = value.ToCharArray();
                array[0] = char.ToUpper(array[0]);
                return new string(array);
            }
            return value;
        }


        //public static Settings ConvertSettings(object v)
        //{
        //    Settings context = new Settings();
        //    if (v != null)
        //    {
        //        context = v (DataContext.Settings);
        //    }
        //    return null;
        //}
    }
}