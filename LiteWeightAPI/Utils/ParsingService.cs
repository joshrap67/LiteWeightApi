using System.Globalization;
using NodaTime;
using NodaTime.Text;

namespace LiteWeightAPI.Utils;

public static class ParsingService
{
    private const string DateFormat = "yyyy-MM-dd";

    public static string ConvertLocalDateToString(LocalDate localDate)
    {
        return localDate.ToString(DateFormat, CultureInfo.InvariantCulture);
    }

    public static string ConvertInstantToString(Instant instant)
    {
        return instant.ToString("g", CultureInfo.InvariantCulture);
    }

    public static LocalDate ParseDateToNodatime(string date)
    {
        var result = LocalDatePattern.Iso.Parse(date);
        if (!result.Success)
        {
            throw new Exception($"Date not in proper format: {DateFormat}");
        }

        return result.Value;
    }
}