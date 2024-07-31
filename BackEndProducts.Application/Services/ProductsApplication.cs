using System.Diagnostics.Metrics;
using System.Text.Json;
using System.Text.Json.Nodes;
using AutoMapper;
using BackEndProducts.Application.Common;
using BackEndProducts.Application.ConfiguracionApi;
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
                        ErrorMessage = "Error: código <ProductId> duplicado"
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
                        ErrorMessage = "Error: try again"
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
                    throw new Exception("Error reading endpoint Country API");


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
                            throw new Exception("Error leyendo data de endpoint de descuentos");
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
                throw new Exception($"Error reading endpoint Country API: --> ${ex.Message}");
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
                    throw new Exception(string.Format("Record cannot modify! Id Product [{0}] was not found!", input.ProductId));
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
                        ErrorMessage = "Error: try again"
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
        /// Validate input for creation record, or update record. Also get de currencie code
        /// </summary>
        /// <param name="action">ADD for create, UPDATE for update</param>
        /// <param name="name"></param>
        /// <param name="salary_per_year"></param>
        /// <param name="type"></param>
        /// <param name="role"></param>
        /// <param name="country"></param>
        /// <returns></returns>
        //private async Task<Product> ValidateInput(ActionType action, string name, int salary_per_year, string type, int? role, string? country)
        //{
        //    string nameMethod = nameof(ValidateInput);
        //    Product dataProduct = new Product();
        //    string currencie_name = null;
        //    string baseUrl = string.Empty;
        //    int? finalRole = null;

        //    // validate name
        //    if (string.IsNullOrEmpty(name))
        //    {
        //        throw new Exception("Error: missing input name");
        //    }

        //    if (name.Trim().Length > 50)
        //        throw new Exception($"Error: the length of [name] field is invalid!");

        //    // validate type
        //    if (string.IsNullOrEmpty(type))
        //    {
        //        throw new Exception("Error: missing input type");
        //    }
        //    // validate if Product type exists

        //    // We validate the Product Type Id
        //    var ProductTypeObject = await _productRepository.GetProductTypeById(type);
        //    if (ProductTypeObject == null)
        //        throw new Exception($"Error: type [{type}] is not valid");

        //    // validate salary
        //    if (salary_per_year < 0)
        //    {
        //        throw new Exception("Error: invalid input salary");
        //    }

        //    // for creation record, validate if the name already exists
        //    if (action == ActionType.ADD && ExistsProductByName(name).Result)
        //        throw new Exception($"Error:: The name [{name}] already exists in the database!");

        //    // For Employee
        //    if (type == BackEndProducts.Common.Enum.ProductType.E.ToString())
        //    {
        //        // if the type is "E" (employe), must validate the role
        //        if (!role.HasValue)
        //        {
        //            throw new Exception($"Error:: role is not valid, is a required field");
        //        }
        //        else if (role.Value <= 0)
        //        {
        //            throw new Exception($"Error:: role value [{role.Value}] is not valid");
        //        }
        //        else
        //        {
        //            // We validate the Product Type Id
        //            var roleTypeObject = await _productRepository.GetRoleTypeById(role.Value);
        //            if (roleTypeObject == null)
        //                throw new Exception($"Error:: role [{role.Value}] does not exist");

        //            // role is OK!
        //            finalRole = role;
        //        }

        //        // if the type is "E" (employe), must validate the Country Field and call API, example: https://restcountries.com/v3.1/name/chile

        //        // validate if the country exists, and must have a lenght > 1 
        //        if (string.IsNullOrEmpty(country))
        //        {
        //            throw new Exception("Error:: country value is required for an Employee Product");
        //        }

        //        if (country.Trim().Length > 40)
        //            throw new Exception($"Error: the length of [country] field is invalid!");

        //        if (country.Length > 1)
        //        {
        //            try
        //            {
        //                baseUrl = _configuration.GetSection("countryConfig").GetSection("endpoint").Value;

        //                if (string.IsNullOrEmpty(baseUrl))
        //                    throw new Exception("Error reading endpoint Country API");
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Error reading endpoint Country API");
        //            }

        //            Api apiCountry = new Api(baseUrl, ConfiguracionApi.CallType.EnumCallType.Get);

        //            //Concatenate the country and call the api

        //            HttpResponseMessage response = await apiCountry.CallApi(country.ToLower());

        //            if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
        //            {
        //                ServiceLog.Write(BackEndProducts.Common.Enum.LogType.WebSite, System.Diagnostics.TraceLevel.Error, nameMethod, $"Readed OK! - Code [" + response.StatusCode + "] Content [" + (response.Content == null ? "NULL" : "documento correcto" + "]"));

        //                if (response.Content != null)
        //                {
        //                    // Try to extract de "currency data" by the Country Field
        //                    try
        //                    {
        //                        string jsonObjectDataResult = await response.Content.ReadAsStringAsync();

        //                        var jsonDocument = JsonDocument.Parse(jsonObjectDataResult);

        //                        // parse from stream, string, utf8JsonReader
        //                        JsonArray? jsonArray = JsonNode.Parse(jsonObjectDataResult)?.AsArray();

        //                        JArray obj = JArray.Parse(jsonObjectDataResult);


        //                        var jsonObject = JObject.Parse(jsonArray[0].ToString());
        //                        var currencieObject = (JObject)jsonObject["currencies"];
        //                        string str = currencieObject.ToString().Replace("\r\n", "").Replace("{", "").Replace("}", "").Replace("\"", "").Trim();

        //                        if (!string.IsNullOrEmpty(str))
        //                        {
        //                            string[] wordSeparatesList = str.Split(":");
        //                            for (int t = 0; t < wordSeparatesList.Length; t++)
        //                            {
        //                                if (wordSeparatesList[t].Trim() == "name")
        //                                {
        //                                    string[] wordSeparatesListComa = wordSeparatesList[t + 1].Split(",");

        //                                    currencie_name = wordSeparatesListComa[0].Trim();
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        currencie_name = null;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        //                {
        //                    //GestorLog.Write(MedicalWebScraping.Common.Enum.LogType.WebSite, TraceLevel.Error, nameMethod, $"Respuesta Error (bad request)! Registro No Insertado! - Sucursal Id [{sucursalItemData.Codigo}] - " + doctorName + "/" + specialityName + "/" + firsthourData + " - Code[" + response.StatusCode + "] Content[" + (response.Content == null ? "NULL" : response.Content.ReadAsStringAsync().Result + "]"));
        //                }
        //                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        //                {
        //                    ServiceLog.Write(BackEndProducts.Common.Enum.LogType.WebSite, System.Diagnostics.TraceLevel.Error, nameMethod, $"Respuesta Error (no autorizado)!: Registro No Insertado! - Code[" + response.StatusCode + "] Content[" + (response.Content == null ? "NULL" : response.Content.ReadAsStringAsync().Result + "]"));
        //                }
        //                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        //                {
        //                    ServiceLog.Write(BackEndProducts.Common.Enum.LogType.WebSite, System.Diagnostics.TraceLevel.Error, nameMethod, $"Respuesta Error (no autorizado)!: Registro No Insertado! - Code[" + response.StatusCode + "] Content[" + (response.Content == null ? "NULL" : response.Content.ReadAsStringAsync().Result + "]"));

        //                }
        //                else
        //                    ServiceLog.Write(BackEndProducts.Common.Enum.LogType.WebSite, System.Diagnostics.TraceLevel.Error, nameMethod, "Respuesta Error!: Content [" + (response.Content == null ? "NULL" : response.Content.ReadAsStringAsync().Result + "]"));

        //            }
        //        }
        //    }

        //    dataProduct.name = name;
        //    dataProduct.salary_per_year = salary_per_year;
        //    dataProduct.type = type;
        //    dataProduct.role = finalRole;
        //    dataProduct.country = country;
        //    dataProduct.currencie_name = currencie_name;

        //    return dataProduct;
        //}

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
                throw new Exception("Error: invalid code");

            ProductDTO outPut = await _productRepository.GetProductById(productId);

            if (outPut == null)
                throw new Exception("Error: Product not found");

            return outPut;

        }
    }
}
