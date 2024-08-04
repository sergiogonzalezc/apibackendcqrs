using System.Diagnostics.Metrics;
using System.Text.Json;
using System.Text.Json.Nodes;
using AutoMapper;
using BackEndProducts.Application.Common;
using BackEndProducts.Application.ConfiguracionApi;
using BackEndProducts.Application.Shared;
using BackEndProducts.Application.Interface;
using BackEndProducts.Application.Model;
using BackEndProducts.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static BackEndProducts.Common.Enum;


namespace BackEndProducts.Application.Services
{
    public class ProductsApplication : IProductApplication
    {
        private IProductRepository _productRepository;
        private IConfiguration _configuration;

        public ProductsApplication(IProductRepository ProductsRepository, IConfiguration configuration)
        {
            _productRepository = ProductsRepository;
            _configuration = configuration;
        }


        /// <summary>
        /// Inserta un nuevo producto
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResultRequestDTO> InsertProduct(InputCreateProduct input)
        {
            string nameMethod = nameof(InsertProduct);
            bool result = false;

            try
            {
                bool itemExist = await _productRepository.ExistsProductById(input.ProductId);

                if (itemExist == true)
                {
                    return new ResultRequestDTO
                    {
                        Success = false,
                        ErrorMessage = DomainErrors.ProductCreationIdDuplicated.message,
                    };
                }

                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<InputCreateProduct, Product>()).CreateMapper();
                Product dataProduct = mapper.Map<InputCreateProduct, Product>(input);

                dataProduct.Discount = await GetDiscountFromApi(input.ProductId) ?? 0;  // asigna descuento nulo si no encuentra el valor en la API

                result = await _productRepository.InsertProduct(dataProduct);

                // generate output
                if (!result)
                {
                    return new ResultRequestDTO
                    {
                        Success = false,
                        ErrorMessage = DomainErrors.ProductCreationGenericError.message,
                    };
                }
                else
                {
                    return new ResultRequestDTO
                    {
                        Success = true,
                        ErrorMessage = null,
                    };
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene el descuento leyebdo una api externa de https://mockapi.io/
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<decimal?> GetDiscountFromApi(int productID)
        {
            try
            {
                string baseUrl = _configuration.GetSection("DiscountApiConfig").GetSection("endpoint").Value;

                if (string.IsNullOrEmpty(baseUrl))
                    throw new Exception(DomainErrors.ReadingConfig.message);


                Api apiCountry = new Api(baseUrl, ConfiguracionApi.CallType.EnumCallType.Get);

                //Concatenate the country and call the api

                HttpResponseMessage response = await apiCountry.CallApi("discounts");

                if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
                {
                    ServiceLog.Write(LogType.WebSite, System.Diagnostics.TraceLevel.Error, nameof(GetDiscountFromApi), $"Api leida OK! - Code [" + response.StatusCode + "] Content [" + (response.Content == null ? "NULL" : "documento correcto" + "]"));

                    if (response.Content != null)
                    {
                        // Try to extract de "currency data" by the Country Field

                        string jsonObjectDataResult = await response.Content.ReadAsStringAsync();

                        if (jsonObjectDataResult != null)
                        {
                            List<DiscoutProductListDTO> businessunits = JsonConvert.DeserializeObject<List<DiscoutProductListDTO>>(jsonObjectDataResult);

                            if (businessunits.Any())
                            {
                                DiscoutProductListDTO? findDiscountValue = businessunits.Where(x => x.productId.Equals(productID)).FirstOrDefault();
                                if (findDiscountValue != null)
                                    return findDiscountValue.discountValue;  // retorna el descuento del producto
                            }
                        }
                        else
                            throw new Exception(DomainErrors.DiscountApiError.message);
                    }
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        //GestorLog.Write(MedicalWebScraping.Common.Enum.LogType.WebSite, TraceLevel.Error, nameMethod, $"Respuesta Error (bad request)! Registro No Insertado! - Sucursal Id [{sucursalItemData.Codigo}] - " + doctorName + "/" + specialityName + "/" + firsthourData + " - Code[" + response.StatusCode + "] Content[" + (response.Content == null ? "NULL" : response.Content.ReadAsStringAsync().Result + "]"));
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        ServiceLog.Write(LogType.WebSite, System.Diagnostics.TraceLevel.Error, nameof(GetDiscountFromApi), $"Respuesta Error (no autorizado)!: Registro No Insertado! - Code[" + response.StatusCode + "] Content[" + (response.Content == null ? "NULL" : response.Content.ReadAsStringAsync().Result + "]"));
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        ServiceLog.Write(LogType.WebSite, System.Diagnostics.TraceLevel.Error, nameof(GetDiscountFromApi), $"Respuesta Error (no autorizado)!: Registro No Insertado! - Code[" + response.StatusCode + "] Content[" + (response.Content == null ? "NULL" : response.Content.ReadAsStringAsync().Result + "]"));

                    }
                    else
                        ServiceLog.Write(LogType.WebSite, System.Diagnostics.TraceLevel.Error, nameof(GetDiscountFromApi), "Respuesta Error!: Content [" + (response.Content == null ? "NULL" : response.Content.ReadAsStringAsync().Result + "]"));

                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{DomainErrors.DiscountApiError.message}: --> ${ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Actualiza un producto
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResultRequestDTO> UpdateProduct(InputUpdateProduct input)
        {
            string nameMethod = nameof(UpdateProduct);
            bool result = false;

            try
            {
                var item = await _productRepository.GetProductById(input.ProductId);

                if (item == null)
                {
                    throw new Exception(string.Format(DomainErrors.ErrorUpdatingProduct.message, input.ProductId));
                }
                else
                {
                    // validate and get de currencie code
                    //Product dataProduct = await ValidateInput(ActionType.UPDATE, input.name, input.salary_per_year, input.type, input.role, input.country);

                    //if (dataProduct == null)
                    //    throw new Exception("Error: try again");

                    //dataProduct.Id = input.ProductId;

                    var mapper = new MapperConfiguration(cfg => cfg.CreateMap<InputUpdateProduct, Product>()).CreateMapper();
                    Product dataProduct = mapper.Map<InputUpdateProduct, Product>(input);

                    result = await _productRepository.UpdateProduct(dataProduct);
                }

                // generate output
                if (!result)
                {
                    return new ResultRequestDTO
                    {
                        Success = true,
                        ErrorMessage = DomainErrors.ProductEditionGenericError.message
                    };
                }
                else
                {
                    return new ResultRequestDTO
                    {
                        Success = true,
                        ErrorMessage = null,
                    };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene la lista completa de productos
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProductDTO>> GetProducts(int pageNumber, int pageSize)
        {

            List<ProductDTO> resultado = await _productRepository.GetProducts(pageNumber, pageSize);
            //if (resultado.Count == 0)
            //    throw new Exception("No existen datos.");

            return resultado;
        }



        /// <summary>
        /// Valida si existe un producto en base a su nombre
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<bool> ExistsProductByName(string name)
        {
            return await _productRepository.ExistsProductByName(name);
        }

        /// <summary>
        /// Valida si existe un producto por id. Devueldve TRUE en caso que exista o FALSE si no existe
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<bool> ExistsProductById(int productId)
        {
            return await _productRepository.ExistsProductById(productId);
        }

        /// <summary>
        /// Get the Product type List filter by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ProductDTO> GetProductsById(int productId)
        {
            if (productId < 0)
                throw new Exception(DomainErrors.ProductCreationIdInvalid.message);

            ProductDTO outPut = await _productRepository.GetProductById(productId);

            if (outPut == null)
                throw new Exception(DomainErrors.ProductCreationNotFound.message);

            return outPut;

        }
    }
}
