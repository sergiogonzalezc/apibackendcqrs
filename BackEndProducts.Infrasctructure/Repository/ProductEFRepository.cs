using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BackEndProducts.Application.Model;
using BackEndProducts.Application.Interface;
using AutoMapper;
using BackEndProducts.Infraestructure.Data;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using BackEndProducts.Sql.Queries;
using BackEndProducts.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Caching.Memory;

namespace BackEndProducts.Infraestructure.Repository
{
    public class ProductEFRepository : IProductRepository
    {
        private readonly string _connString;
        private readonly IConfiguration _configuracion;
        private DBContextProducts _dataBaseDBContext;
        private Mapper _mapper;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IMemoryCache _memoryCache;
        private IWebHostEnvironment _currentEnvironment { get; }
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);  // Se asignan 5 minutos de duracion de CACHE

        public ProductEFRepository(IConfiguration configuracion, IWebHostEnvironment env, IMemoryCache memoryCache)
        {
            _currentEnvironment = env;
            _configuracion = configuracion;
            _connString = _configuracion.GetConnectionString("stringConnection");

            //ServiceLog.Write(BackEndProducts.Common.Enum.LogType.WebSite, System.Diagnostics.TraceLevel.Info, nameof(ProductEFRepository), $"CONEXION A BD por defecto [{_connString}...");

            var opcionesDBContext = new DbContextOptionsBuilder<DBContextProducts>();
            //opcionesDBContext.UseMySQL(_cadenaConexion);

            // Si es desarrollo, se usa la BD local. En cambio, si es producción, se usa la BD de DOCKER
            if (!_currentEnvironment.IsDevelopment())
            {
                var servidorbd = Environment.GetEnvironmentVariable("DB_SERVER_HOST") ?? @"THEKONES-PC\\SQLEXPRESS";
                var puerto = Environment.GetEnvironmentVariable("DB_SERVER_PORT") ?? @"1433";
                var basedatos = Environment.GetEnvironmentVariable("DB_NAME");
                var user = Environment.GetEnvironmentVariable("DB_USER");
                var contrasenna = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");

                //_connString = $"Server={servidorbd},{puerto};Initial Catalog={basedatos};User ID={user};Password={contrasenna};TrustServerCertificate=true;";
                _connString = $"Data Source={servidorbd};Initial Catalog={basedatos};User ID={user};Password={contrasenna};TrustServerCertificate=true;Encrypt=False";
            }

            opcionesDBContext.UseSqlServer(_connString);

            //ServiceLog.Write(BackEndProducts.Common.Enum.LogType.WebSite, System.Diagnostics.TraceLevel.Info, nameof(ProductEFRepository), $"NEW CONEXION A BD [{_connString}...");


            _dataBaseDBContext = new DBContextProducts(opcionesDBContext.Options, _currentEnvironment);

            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProductEF, Product>().ReverseMap();
            }
            );

            _mapper = new Mapper(config);
            _memoryCache = memoryCache;
        }

        #region Product

        /// <summary>
        /// Inserta un nuevo producto
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> InsertProduct(Product input)
        {
            try
            {
                ProductEF ProductBD = _mapper.Map<ProductEF>(input);

                _dataBaseDBContext.ProductEF.Add(ProductBD);
                bool result = await _dataBaseDBContext.SaveChangesAsync() > 0;

                return result;
            }
            catch (Exception ex)
            {
                //ServiceLog.Write(BackEndProducts.Common.Enum.LogType.WebSite, ex, "Error", $"===> CONector [{_dataBaseDBContext.Database.GetConnectionString}]...");

                throw;
            }
        }


        /// <summary>
        /// Valida si existe un producto por su nombre
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<bool> ExistsProductByName(string name)
        {
            List<ProductEF>? dataBD = await _dataBaseDBContext.ProductEF.Where(x => x.Name.ToUpper().Equals(name.ToUpper())).ToListAsync();
            if (dataBD == null || dataBD.Count == 0)
                return false;
            else
                return true;
        }


        /// <summary>
        /// Valida si existe un producto por id. Devueldve TRUE en caso que exista o FALSE si no existe
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<bool> ExistsProductById(int productId)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(_connString))
                {
                    connection.Open();
                    var result = await connection.QuerySingleOrDefaultAsync<ProductDTO>(ProductQueries.AllProductById, new { ProductId = productId });

                    return (result != null);
                }
            }
            catch (Exception ex)
            {
                //ServiceLog.Write(BackEndProducts.Common.Enum.LogType.WebSite, ex, "Error", $"===> CONector [{_dataBaseDBContext.Database.GetConnectionString}]...");

                throw;
            }
        }

        /// <summary>
        /// Obtiene lista de productos en base a la paginación
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProductDTO>> GetProducts(int pageNumber, int pageSize)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(_connString))
                {
                    connection.Open();
                    var result = await connection.QueryAsync<ProductDTO>(ProductQueries.AllProduct, new { @PageSize = pageSize, PageNumber = pageNumber });

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                // ServiceLog.Write(BackEndProducts.Common.Enum.LogType.WebSite, ex, nameof(GetProducts), $"===> Error [{_connString}]...");

                throw;
            }
        }



        /// <summary>
        /// Obtiene un producto en base a su ID
        /// </summary>
        /// <returns></returns>
        public async Task<ProductDTO> GetProductById(int productId)
        {
            //ProductDTO productDTO = null;
            try
            {
                string cacheKey = $"product-{productId}";

                // Si no encontramos el producto en el caché, lo ingresamos al cache , con un tiempo de expiración de por 5 minutos 
                if (!_memoryCache.TryGetValue(cacheKey, out ProductDTO productDTO))
                {
                    using (IDbConnection connection = new SqlConnection(_connString))
                    {
                        connection.Open();
                        productDTO = await connection.QuerySingleOrDefaultAsync<ProductDTO>(ProductQueries.AllProductById, new { ProductId = productId });

                        if (productDTO != null)
                        {
                            var cacheEntryOptions = new MemoryCacheEntryOptions()
                                            //.SetSlidingExpiration(_cacheDuration)
                                            .SetAbsoluteExpiration(_cacheDuration)
                                            .SetPriority(CacheItemPriority.Normal);

                            _memoryCache.Set(cacheKey, productDTO, cacheEntryOptions);

                            productDTO.Status = 1;
                        }
                    }
                }
                else
                    productDTO.Status = 0;

                return productDTO;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Actualiza un producto
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> UpdateProduct(Product input)
        {
            try
            {
                ProductEF? ProductBD = await _dataBaseDBContext.ProductEF.FirstOrDefaultAsync(m => m.ProductId == input.ProductId);
                {
                    ProductBD.Name = input.Name;
                    ProductBD.Status = input.Status;
                    ProductBD.Stock = input.Stock;
                    ProductBD.Description = input.Description;
                    ProductBD.Price = input.Price;
                }

                _dataBaseDBContext.ProductEF.Update(ProductBD);
                bool result = await _dataBaseDBContext.SaveChangesAsync() > 0;

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion



    }
}