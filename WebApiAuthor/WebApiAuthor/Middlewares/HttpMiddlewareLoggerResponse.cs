namespace WebApiAuthor.Middlewares;


public static class HttpMiddlewareLoggerResponseExtensions
{
     public static IApplicationBuilder UseHttpLogger(this IApplicationBuilder app)
     {
          return app.UseMiddleware<HttpMiddlewareLoggerResponse>();
     }
}
public class HttpMiddlewareLoggerResponse
{
     //Logica centralizada para poder guardar todo lo que vayamos a enviar en el cuarpo de la respuesta hacia nuestros clientes
     //Va a funcionar con cualquier ruta
     //Esto es un middleWare personalizado
     
     private readonly RequestDelegate _next;
     private readonly ILogger<HttpMiddlewareLoggerResponse> _logger;

     public HttpMiddlewareLoggerResponse(RequestDelegate next, ILogger<HttpMiddlewareLoggerResponse> logger)
     {
          _next = next;
          _logger = logger;
     }
     
     //Invoke or InvokeAsync

     public async Task InvokeAsync(HttpContext context)
     {
          using (var ms = new MemoryStream())
          {
               var originalBodyResponse = context.Response.Body;
               context.Response.Body = ms;

               await _next(context);

               ms.Seek(0, SeekOrigin.Begin);
               string response = new StreamReader(ms).ReadToEnd();
               ms.Seek(0, SeekOrigin.Begin);

               await ms.CopyToAsync(originalBodyResponse);
               context.Response.Body = originalBodyResponse;

               _logger.LogInformation(response);
          }
     }
}