﻿using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApiAuthor.Filters;
using WebApiAuthor.Middlewares;
using WebApiAuthor.Services;
using WebApiAuthor.Utilities;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]

namespace WebApiAuthor
{
    public class Startup
{
    public Startup(IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add(typeof(ExceptionFilter));
            options.Conventions.Add(new SwaggerGroupVersion());
        }).AddJsonOptions
            (x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
                ClockSkew = TimeSpan.Zero
            }); //Sistema de Autentificacion

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "WebApisCourse",
                Version = "v1",
                Description = "Authors and Books Web Api",
                Contact = new OpenApiContact
                {
                    Email = "roman@7r1ck.com",
                    Name = "Roman Vitolo",
                    Url = new Uri("http://www.trickgs.com")
                },
                License = new OpenApiLicense
                {
                    Name = "License: MIT"
                }
            });
            c.SwaggerDoc("v2", new OpenApiInfo { Title = "WebApisCourse", Version = "v2" });
            c.OperationFilter<AddHATEOASParameters>();
            c.OperationFilter<AddVersionParameters>();

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                },
            });
            var XMLFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var XMLPath = Path.Combine(AppContext.BaseDirectory, XMLFile);
            c.IncludeXmlComments(XMLPath);
        });
        services.AddAutoMapper(typeof(Startup));

        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("IsAdmin", policy =>
                policy.RequireClaim("IsAdmin"));
            options.AddPolicy("IsSeller", policy =>
                policy.RequireClaim("IsSeller"));
        });

        services.AddDataProtection();

        services.AddTransient<HashService>();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins("https://apirequest.io").AllowAnyMethod().AllowAnyHeader()
                    .WithExposedHeaders(new string[] { "totalAmountRecords" });
            });
        });

        services.AddTransient<LinksGenerator>();
        services.AddTransient<HATEOASAuthorFilterAttribute>();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        services.AddApplicationInsightsTelemetry(Configuration["ApplicationInsights:ConnectionString"]);
    }

    //Todos los Middleware, se ejecutan en orden. Los middleware son los que dicen "Use"
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        app.UseHttpLogger();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

        }

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint
                ("/swagger/v1/swagger.json", "WebApisCourse v1");
            c.SwaggerEndpoint
                ("/swagger/v2/swagger.json", "WebApisCourse v2");
        });

        //Al sacar del IF la linea 49, podriamos tenerlo en produccion

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
}
