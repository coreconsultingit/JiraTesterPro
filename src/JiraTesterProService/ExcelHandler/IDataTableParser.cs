
namespace JiraTesterProService.ExcelHandler
{


    public interface IDataTableParser<T> where T : new()
    {
        (IList<T> lstItems, IList<string> lstValidationMessage) ConvertDataTableToList(DataTable dt, Action<T> act);
    }
    public class DataTableParser<T> : IDataTableParser<T> where T : new()
    {
        private IList<string> ValidationMessages { get; set; }
        private IPropertyBinder propertyBinder;
        public DataTableParser(IPropertyBinder propertyBinder)
        {
            this.propertyBinder = propertyBinder;
        }


        public (IList<T> lstItems, IList<string> lstValidationMessage) ConvertDataTableToList(DataTable dt, Action<T> act)
        {
            ValidationMessages = new List<string>();

            var valueProvider = new DataTableValueProvider<T>(dt);
            var binder = new CollectionBinder<T>(propertyBinder);
            var sourceItems = binder.Bind(valueProvider, act).ToArray();


            foreach (var message in binder.ValidationMessages)
            {
                ValidationMessages.Add(message);
            }



            return (sourceItems, ValidationMessages);



        }
    }

}
