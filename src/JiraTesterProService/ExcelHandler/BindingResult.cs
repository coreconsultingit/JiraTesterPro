namespace JiraTesterProService.ExcelHandler
{
    public class BindingResult<TEntity>

    {

        public TEntity Entity { get; private set; }

        public bool Valid { get; private set; }

        public IEnumerable<string> ValidationMessages { get; private set; }



        public BindingResult(TEntity entity, bool valid, IEnumerable<string> validationMessages)

        {

            Entity = entity;

            Valid = valid;

            ValidationMessages = validationMessages;

        }

    }
}