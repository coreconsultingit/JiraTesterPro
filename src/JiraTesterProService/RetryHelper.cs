namespace JiraTesterProService;

public interface IRetryHelper
{
    void Execute(Action action, int numberOfRetries);
    Task<T> Retry<T>(Func<Task<T>> func, int numberOfRetries);
}

public class RetryHelper : IRetryHelper
     {
         private ILogger<RetryHelper> logger;

         public RetryHelper(ILogger<RetryHelper> logger)
         {
             this.logger = logger;
         }


         public void Execute(Action action, int numberOfRetries)
         {
             var tries = 0;
             while (tries <= numberOfRetries)
             {
                 try
                 {
                     action();
                     return;
                 }
                 catch (Exception ex)
                 {
                     logger.LogError(ex.Message);
                     logger.LogError(ex.StackTrace);
                     Thread.Sleep(10000);
                     tries++;
                 }
             }

             throw new Exception($"Error after {tries} tries");
         }

         public async Task<T> Retry<T>(Func<Task<T>> func, int numberOfRetries)
         {
             var tries = 0;

             while (tries <= numberOfRetries)
             {
                 try
                 {
                     return await func();
                 }
                 catch (Exception ex)
                 {
                     logger.LogError(ex.Message);
                     logger.LogError(ex.StackTrace);

                     logger.LogInformation($"Going to retry in 5000 ms");
                     await Task.Delay(10000);
                     tries++;
                 }
             }

             throw new Exception("Failed rerunning");
         }
     }
