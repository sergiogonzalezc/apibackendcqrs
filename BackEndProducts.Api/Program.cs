using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using BackEndProducts.Application.Interface;
using BackEndProducts.Application.Services;
using BackEndProducts.Common;
using BackEndProducts.Infraestructure;
using BackEndProducts.Infraestructure.Repository;
using BackEndProducts.Api.Endpoints;
using BackEndProducts.Api.Model;
using Carter;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using NLog;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using FluentValidation;
using System.Configuration;
using static Org.BouncyCastle.Math.EC.ECCurve;
using BackEndProducts.Application.Behaviors;
using BackEndProducts.Application.Exceptions.Handler;
using Autofac.Core;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using static BackEndProducts.Application.Exceptions.Handler.CustomExceptionHandler;
using FluentAssertions.Common;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

string envName = builder.Environment.EnvironmentName;

if (envName != "Development" && envName != "qa")
{
    builder.Services.AddTransient<ProblemDetailsFactory, CustomProblemDetailsFactory>();
}

// Add services to the container.
builder.Services.AddScoped<IProductApplication, ProductsApplication>();
builder.Services.AddScoped<IProductRepository, ProductEFRepository>();
builder.Services.AddScoped(typeof(IProductApplication), typeof(ProductsApplication));
builder.Services.AddScoped(typeof(IProductRepository), typeof(ProductEFRepository));

//string connectionStringsData = builder.Configuration.GetSection("ConnectionStrings").GetValue<string>("stringConnection");

//builder.Services.AddDbContext<DBContextProducts>(x => x.UseSqlServer(connectionStringsData, x => x.EnableRetryOnFailure()));
builder.Services.AddDbContext<DBContextProducts>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("localhost", builder =>
    {
        builder.WithOrigins("http://localhost:3000",
                            "http://localhost:3001",
                            "http://localhost:3002",
                            "http://localhost:3003",
                            "http://localhost:12000",
                            "http://localhost:8080",
                            "http://localhost:8081",
                            "http://localhost:8082",
                            "http://localhost:5000",
                            "http://localhost:25471",
                            "http://localhost:57344",
                            "http://localhost:57343",
                            "http://localhost:80"
                            )
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
        //.WithExposedHeaders("content-disposition");
    });
});


//builder.Services.AddOpenApiDocument(document =>
//{
//    document.Title = "API de Consulta de productos!";
//    document.Description = "Esta api permite insertar, leer registros por paginacion, leer registros por su ID, y actualizar un producto.";
//});


builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerDocument();
//builder.Services.AddSwaggerGen();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ErrorHandlingFilterAttribute>();
});

foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
{
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assembly)
                                        .AddOpenBehavior(typeof(ValidationBehavior<,>))
                                        .AddOpenBehavior(typeof(LoggingBehavior<,>))
    );

    builder.Services.AddValidatorsFromAssembly(assembly);
}

builder.Services.AddCarter();

builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
});

builder.Services.AddOptions();

builder.Services.AddMemoryCache();

//Cross-Cutting Services
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API de Consulta de productos",
        Description = "Api Rest que permite insertar, leer registros por paginación, leer registros por su ID, y actualizar un producto",       
        Contact = new OpenApiContact
        {
            Name = "Datos de Contacto",
            Email = "sergio.gonzalez.c@gmail.com"
           // Url = new Uri("https://example.com/contact").ToString()
        }        
    });
});


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    //Solo habilitado en producción
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseHsts();
}

// define culture spanish CL
var cultureInfo = new CultureInfo("es-CL");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(cultureInfo),
    SupportedCultures = new List<CultureInfo>
                {
                    cultureInfo,
                },
    SupportedUICultures = new List<CultureInfo>
                {
                    cultureInfo,
                }
});

//app.UseMiddleware<ValidationExceptionHandlingMiddleware>();

app.UseCors("localhost");
app.UseAuthentication();
app.UseAuthorization();

// SGC - Asigna la version del Assembly al Log4Net
Assembly thisApp = Assembly.GetExecutingAssembly();

AssemblyName name = new AssemblyName(thisApp.FullName);

// Identifica la versión del ensamblado
GlobalDiagnosticsContext.Set("VersionApp", name.Version.ToString());

// Identifica el nombre del ensamblado para poder identificar el ejecutable
GlobalDiagnosticsContext.Set("AppName", name.Name);

// Obtiene el process ID
Process currentProcess = Process.GetCurrentProcess();

GlobalDiagnosticsContext.Set("ProcessID", "PID " + currentProcess.Id.ToString());


app.Map("/error", () =>
                {
                    ServiceLog.Write(BackEndProducts.Common.Enum.LogType.WebSite, System.Diagnostics.TraceLevel.Error, "INICIO_API", "An Error Occurred...!!");

                    throw new InvalidOperationException("An Error Occurred...");
                });

app.UseRouting();
app.UseResponseCaching();
app.UseHttpsRedirection();
app.MapControllers();
app.MapCarter();
app.UseExceptionHandler(options => { });

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

ServiceLog.Write(BackEndProducts.Common.Enum.LogType.WebSite, System.Diagnostics.TraceLevel.Info, "INICIO_API", "===== INICIO API [BackEndProducts.Api] =====");

app.Run();
