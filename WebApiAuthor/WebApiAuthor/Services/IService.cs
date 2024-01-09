namespace WebApiAuthor.Services;

public interface IService
{
     Guid GetTransient();
     Guid GetScoped();
     Guid GetSingleton();
     void DoTask();
     
}
     public class ServiceA : IService
     {
          private readonly ILogger<ServiceA> _logger;
          private readonly TransientService _transientService;
          private readonly ScopeService _scopeService;
          private readonly SingletonService _singletonService;

          public ServiceA(ILogger<ServiceA> logger, TransientService transientService,
               ScopeService scopeService, SingletonService singletonService)
          {
               _logger = logger;
               _transientService = transientService;
               _scopeService = scopeService;
               _singletonService = singletonService;
          }

          public Guid GetTransient() { return _transientService.Guid;}
          public Guid GetScoped() { return _scopeService.Guid;}
          public Guid GetSingleton() { return _singletonService.Guid;}
          
          public void DoTask()
          {
             
          }
     }
     
     public class ServiceB : IService
     {
          public Guid GetTransient()
          {
               throw new NotImplementedException();
          }

          public Guid GetScoped()
          {
               throw new NotImplementedException();
          }

          public Guid GetSingleton()
          {
               throw new NotImplementedException();
          }

          public void DoTask()
          {
               
          }
     }

     public class TransientService
     {
          public Guid Guid = Guid.NewGuid();
     }
     
     public class ScopeService
     {
          public Guid Guid = Guid.NewGuid();
     }
     
     public class SingletonService
     {
          public Guid Guid = Guid.NewGuid();
     }
