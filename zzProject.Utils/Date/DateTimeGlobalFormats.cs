using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace zzProject.Utils.Date
{
    public class DateTimeGlobalFormats
    {
        public static List<string> GetSpecificShortDateFormats(string format, System.Globalization.CultureInfo culture)
        {
            return GetShortDateAlternativeFormats(format, culture.DateTimeFormat.DateSeparator);
        }

        public static List<string> GetPrincipalShortDateFormats(System.Globalization.CultureInfo culture)
        {
            return GetShortDateAlternativeFormats(culture.DateTimeFormat.ShortDatePattern, culture.DateTimeFormat.DateSeparator);
        }

        public static List<string> GetAlternativeShortDateFormats(System.Globalization.CultureInfo culture)
        {
            var result = new List<string>();
            foreach (string format in culture.DateTimeFormat.GetAllDateTimePatterns('d'))
	        {
                List<string> newFormats = GetShortDateAlternativeFormats(format, culture.DateTimeFormat.DateSeparator);
                foreach (string newFormat in newFormats)
                {
                    if (result.Where(x => x == newFormat).Count() == 0) result.Add(newFormat);
                }
	        }
            return result;
        }

        private static List<string> GetShortDateAlternativeFormats(string format, string separator)
        {
            var result = new List<string>(){format};
            result = Replace(result, separator, "y", "yy");
            //result = Replace(result, separator, "yy", "yyyy");
            result = Replace(result, separator, "yyy", "yyyy");
            result = Replace(result, separator, "yyyyy", "yyyy");
            result = Replace(result, separator, "MMM", "MM");
            result = ReplaceAndExpand(result, separator, "dd", "d");
            result = ReplaceAndExpand(result, separator, "d", "dd");
            result = ReplaceAndExpand(result, separator, "MM", "M");
            result = ReplaceAndExpand(result, separator, "M", "MM");
            result = ReplaceAndExpand(result, separator, "yyyy", "yy");
            result = ReplaceAndExpand(result, separator, "yy", "yyyy");
            //TODO: make all the necessary Replace and ReplaceAndExpand calls for every format ("MM","M"), etc.
            return result;
        }

        private static List<string> Replace(List<string> formats, string separator, string searchEx, string replaceEx)
        {
            string regularExpression = "[^|\\" + separator + "]" + searchEx + "[^$|^\\" + separator + "]";
            List<string> resultFormats = new List<string>(formats);
            for (int i = 0; i < resultFormats.Count(); i++)
            {
                if (Regex.Match(formats[i], regularExpression).Success)
                {
                    formats[i] = Regex.Replace(formats[i], regularExpression, replaceEx);
                }
            }
            return resultFormats;
        }

        private static List<string> ReplaceAndExpand(List<string> formats, string separator, string searchEx, string replaceEx)
        {
            string regularExpression = "(?<=" + Regex.Escape(separator) + "|^)" + Regex.Escape(searchEx) + "(?=" + Regex.Escape(separator) + "|$)";
            List<string> resultFormats = new List<string>(formats);
            int i = 0;
            int total = resultFormats.Count();
            while (i < total)
            {
                if (Regex.Match(resultFormats[i], regularExpression).Success)
                {
                    string newFormat = Regex.Replace(resultFormats[i], regularExpression, Regex.Escape(replaceEx));
                    if (resultFormats.Where(x => x == newFormat).Count() == 0)
                    {
                        resultFormats.Add(newFormat);
                    }
                }
                i += 1;
                total = resultFormats.Count();
            }
            return resultFormats;
        }
    }
}
