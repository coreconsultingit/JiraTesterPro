namespace JiraTesterProService.ExcelHandler
{
    public class CollectionBinder<TEntity> where TEntity : new()

    {

        public List<string> ValidationMessages { get; private set; }

        private readonly IPropertyBinder binder;



        public CollectionBinder(IPropertyBinder binder)

        {

            this.binder = binder;

        }



        public IEnumerable<TEntity> Bind(IValueProvider valueProvider, Action<TEntity> act)

        {

            ValidationMessages = new List<string>();

            var entities = new List<TEntity>();

            int rowNumber = 1;

            foreach (var value in valueProvider.GetValues())

            {

                var entity = new TEntity();

                var bindingResult = binder.Bind(entity, value);

                if (bindingResult.Valid)

                {
                    if (act != null)
                    {
                        act.Invoke(entity);
                    }
                    entities.Add(entity);



                }

                else

                {

                    ValidationMessages.AddRange(bindingResult.ValidationMessages.Select(x => string.Format("Row {0}: {1}", rowNumber, x)));

                }

                rowNumber++;

            }

            return entities;

        }

    }
}