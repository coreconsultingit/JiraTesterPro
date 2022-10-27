namespace JiraTesterProService.ExcelHandler
{
    public interface IPropertyBinder
    {
        BindingResult<TEntity> Bind<TEntity>(TEntity entity, IDictionary<string, object> values);

    }
}