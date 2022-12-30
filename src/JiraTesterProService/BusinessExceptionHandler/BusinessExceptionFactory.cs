namespace JiraTesterProService.BusinessExceptionHandler;

public class BusinessExceptionFactory: IBusinessExceptionFactory
{
    public BusinessExceptionFactory()
    {
        BusinessExceptionList= new List<BusinessException>();
    }

    private IList<BusinessException> BusinessExceptionList;

    public void AddBusinessException(BusinessException businessException)
    {
        BusinessExceptionList.Add(businessException);
    }

    public IList<BusinessException> GetBusinessExceptionList()
    {
        return BusinessExceptionList;
    }

    public void ClearException()
    {
        BusinessExceptionList.Clear();
    }
}