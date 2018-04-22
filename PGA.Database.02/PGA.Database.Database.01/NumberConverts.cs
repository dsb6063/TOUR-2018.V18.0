#region

using System.Globalization;
using System.Text.RegularExpressions;

#endregion

namespace PGA.Database
{
    public static class NumberConverts
    {
        // From PHP documentation for is_numeric
        // (http://php.net/manual/en/function.is-numeric.php)

        // Finds whether the given variable is numeric.

        // Numeric strings consist of optional sign, any number of digits, optional decimal part and optional
        // exponential part. Thus +0123.45e6 is a valid numeric value.

        // Hexadecimal (e.g. 0xf4c3b00c), Binary (e.g. 0b10100111001), Octal (e.g. 0777) notation is allowed too but
        // only without sign, decimal and exponential part.
        static readonly Regex IsNumericRegex =
            new Regex("^(" +
                      /*Hex*/ @"0x[0-9a-f]+" + "|" +
                      /*Bin*/ @"0b[01]+" + "|" +
                      /*Oct*/ @"0[0-7]*" + "|" +
                      /*Dec*/ @"((?!0)|[-+]|(?=0+\.))(\d*\.)?\d+(e\d+)?" +
                      ")$");

        public static bool IsNumeric(string value)
        {
            return IsNumericRegex.IsMatch(value);
        }
    }

    /// <summary>
    ///     Class StringHelpers.
    /// </summary>
    public static class StringHelpers
    {
        /// <summary>
        ///     checks the string for "all" text and its length for the name validation.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns><c>true</c> if there is "All" match, or length > 9 <c>false</c> otherwise.</returns>
        public static bool FileNameValidation(string filename)
        {
            var culture = new CultureInfo("en-us");

            var failed = false;
            if (filename.Length > 9)
                failed = true;
            else if (culture.CompareInfo.IndexOf(filename, "All", CompareOptions.IgnoreCase) >= 0)
                failed = true;
            return failed;
        }
    }
}