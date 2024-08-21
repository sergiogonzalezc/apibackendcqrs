//using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using BackEndProducts.Infraestructure.Repository;
using BackEndProducts.Application.Model;
using System.Configuration;
using Microsoft.Extensions.Options;
using BackEndProducts.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Autofac.Core;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Hosting;
using FluentAssertions.Common;
using System.IO;
using System.Reflection;

namespace BackEndProducts.Infraestructure
{
    public class DBContextProducts : DbContext
    {
        private IWebHostEnvironment _currentEnvironment { get; }
        private readonly IConfiguration _configuracion;

        private string _connString { get; set; }

        public DbSet<ProductEF> ProductEF { get; set; }

        private DBContextProducts()
        {
            _connString = String.Empty;
        }

        public DBContextProducts(DbContextOptions<DBContextProducts> dbContextOptions, string cadenaConexion) : base(dbContextOptions)
        {
            _connString = cadenaConexion;
        }
        public DBContextProducts(IConfiguration configuracion, DbContextOptions<DBContextProducts> dbContextOptions, IWebHostEnvironment env) : base(dbContextOptions)
        {
            try
            {
                _currentEnvironment = env;
                _configuracion = configuracion;
                //_connString = Database.GetDbConnection().ConnectionString ?? String.Empty;

            }
            catch (Exception ex)
            {
                ServiceLog.Write(BackEndProducts.Common.Enum.LogType.WebSite, ex, nameof(DBContextProducts), "Error instanciando contexto!");
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                if (!optionsBuilder.IsConfigured)
                {
                    // Si es desarrollo, se usa SQL local. En cambio, si es producción se usa SQL del contenedor de DOCKER
                    if (!_currentEnvironment.IsDevelopment())
                    {
                        // var password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD");
                        var servidorbd = Environment.GetEnvironmentVariable("DB_SERVER_HOST") ?? @"THEKONES-PC\\SQLEXPRESS";
                        var puerto = Environment.GetEnvironmentVariable("DB_SERVER_PORT") ?? @"1433";
                        var basedatos = Environment.GetEnvironmentVariable("DB_NAME");
                        var user = Environment.GetEnvironmentVariable("DB_USER");
                        var contrasenna = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");

                        _connString = $"Server={servidorbd},{puerto};Initial Catalog={basedatos};User ID={user};Password={contrasenna};TrustServerCertificate=true";
                    }
                    else
                    {
                        //   _connString = Database.GetDbConnection().ConnectionString ?? String.Empty;
                        //string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                        //ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
                        //IConfigurationRoot builder = configurationBuilder.SetBasePath(path)
                        //                                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        //                                   .Build();

                        //_connString = builder.GetConnectionString("stringConnection");                        

                        _connString = _configuracion.GetConnectionString("stringConnection");
                    }
                    optionsBuilder.UseSqlServer(_connString);
                }
            }
            catch (Exception ex)
            {
                ServiceLog.Write(BackEndProducts.Common.Enum.LogType.WebSite, ex, nameof(OnConfiguring), "Error inicio!");
            }

        }
    }
}