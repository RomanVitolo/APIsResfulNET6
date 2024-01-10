using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebApiAuthor.Filters;
using WebApiAuthor.Middlewares;    

namespace WebApiAuthor;

public class Startup
{
    public Startup(IConfiguration configuration)
    {    
        Configuration = configuration;
    }
    
    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add(typeof(ExceptionFilter));
        }).AddJsonOptions
            (x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));     
        

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);  //Sistema de Autentificacion
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo {Title = "WebApisCourse", Version = "v1"});
        });
    }

    
    //Todos los Middleware, se ejecutan en orden. Los middleware son los que dicen "Use"
    //
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        app.UseHttpLogger();  
        
        
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseSwaggerUI(c => c.SwaggerEndpoint
                ("/swagger/v1/swagger.json", "WebApisCourse v1"));
        }
        
        //Al sacar del IF la linea 49, podriamos tenerlo en produccion

        app.UseHttpsRedirection();

        app.UseRouting();    

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}