using BackEndProducts.Application.Common;
using BackEndProducts.Application.Model;

namespace BackEndProducts.Application.Interface
{
    public interface IProductApplication
    {
        public Task<ResultRequestDTO> InsertProduct(InputCreateProduct input);


        /// <summary>
        /// Actualiza un producto
        /// </summary>
        /// <returns></returns>
        Task<ResultRequestDTO> UpdateProduct(InputUpdateProduct input);


        /// <summary>
        /// Obtiene la lista de productos
        /// </summary>
        /// <returns></returns>
        Task<List<ProductDTO>> GetProducts(int pageNumber, int pageSize);


        /// <summary>
        /// Obtiene un producto por su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ProductDTO> GetProductsById(int id);

        /// <summary>
        /// Valida si existe un producto en base a su nombre
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<bool> ExistsProductByName(string name);

        /// <summary>
        /// Valida si existe un producto por id. Devueldve TRUE en caso que exista o FALSE si no existe
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task<bool> ExistsProductById(int productId);

    }
}