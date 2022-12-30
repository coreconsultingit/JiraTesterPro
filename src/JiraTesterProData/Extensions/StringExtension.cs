using System.Globalization;

namespace JiraTesterProData.Extensions;

public static class StringExtension
{
    public static string ReplaceLastOccurrence(this string source, string find, string replace)
    {
        int place = source.LastIndexOf(find);

        if (place == -1)
            return source;

        return source.Remove(place, find.Length).Insert(place, replace);
    }

    public static IList<string> GetStringSplitOnNewLine(this string source, string[] arrDelimeter=null)
    {
        if (string.IsNullOrEmpty(source))
        {
            return new List<string>();
        }

        if (arrDelimeter == null)
        {
            arrDelimeter = new string[] { "\r\n", "\n" };
        }

        return source.Split(arrDelimeter, StringSplitOptions.None);
    }
    public static string GetNoneIfEmptyOrNull(this string val)
    {
        if (string.IsNullOrEmpty(val))
        {
            return "None";
        }

        return val;
    }
    public static string GetEmptyIfEmptyOrNull(this string val)
    {
        if (string.IsNullOrEmpty(val))
        {
            return String.Empty;
        }

        return val;
    }

    public static string GetNoneIfEmptyOrNull(this object val)
    {
        if (val == null)
        {
            return "None";
        }

        return val.ToString();
    }

    public static string GetEmptyIfEmptyOrNull(this object val)
    {
        if (val == null)
        {
            return "None";
        }

        return val.ToString();
    }
    public static string StandardiseColumnTableName(this string name)
    {

        return name.Replace("[", "").Replace("]", "").Replace("|", "_").Replace(";", "_")
            .Replace(",", "_").Replace(" ", "").Replace("#", "_").Replace("-", "_").Replace("%", "_")
            .Replace(".", "_").Trim();
    }

    public static bool EqualsWithIgnoreCase(this string val, string compareval)
    {
        if (string.IsNullOrEmpty(compareval) || string.IsNullOrEmpty(val))
        {
            return false;

        }

        return val.Equals(compareval, StringComparison.OrdinalIgnoreCase);
    }

    

    public static (string, decimal) ParseCurrencyWithSymbol(this string input)
    {
        var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
            .GroupBy(c => c.NumberFormat.CurrencySymbol)
            .ToDictionary(c => c.Key, c => c.First());


        var culture = cultures.FirstOrDefault(c => input.Contains(c.Key));
        var currency = string.Empty;
        decimal result = 0;
        if (!culture.Equals(default(KeyValuePair<string, CultureInfo>)))
        {
            currency = culture.Key.ToString();

            result = decimal.Parse(input.Replace(culture.Key.ToString(), ""));
        }
        else
        {
            if (!decimal.TryParse(input, out result))
            {
                throw new Exception("Invalid number format");
            }
        }

        return (currency, result);
    }


    public static bool ContainsWithIgnoreCase(this string val, string valtocompare)
    {
        if (string.IsNullOrEmpty(valtocompare) || string.IsNullOrEmpty(val))
        {
            return false;
        }

        return val.Contains(valtocompare, StringComparison.OrdinalIgnoreCase);
    }
}