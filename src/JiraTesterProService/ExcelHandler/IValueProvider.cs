namespace JiraTesterProService.ExcelHandler
{
    public interface IValueProvider
    {
        IEnumerable<IDictionary<string, object>> GetValues();

    }
}