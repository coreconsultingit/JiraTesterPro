using System.Text;

namespace JiraTesterProData.Extensions;

public static class DateTimeExtension
{
    public static string GetJiraFormatDateTime(this DateTime? source)
    {
        if (source == null)
        {
            return DateTime.Now.ToString("d/MMM/yy");
        }

        return source.Value.ToString("d/MMM/yy");
    }
    public static string GetJiraFormatDateTime(this DateTime source)
    {
        
        return source.ToString("d/MMM/yy");
    }
    public static string GetDisplayFormatDateTime(this DateTime source)
    {
        return source.ToString("ddMMMyyyy HH:mm:ss");
    }

    public static Guid GenerateGuid(this DateTime dateTime, string groupCode)
    {
        var bytes = BitConverter.GetBytes(dateTime.Ticks).Union(Encoding.ASCII.GetBytes(groupCode)).ToArray();
        
        Array.Resize(ref bytes, 16); 
        
        var guid = new Guid(bytes);
        return guid;

    }
}