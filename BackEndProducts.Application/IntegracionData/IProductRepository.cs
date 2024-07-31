using BackEndProducts.Application.Common;
using BackEndProducts.Application.Model;

namespace BackEndProducts.Application.Interface
{
    public interface IProductRepository
    {
        public Task<bool> InsertProduct(Product input);

        /// <summary>
        /// Actualiza un producto
        /// </summary>
        /// <returns></returns>
        public Task<bool> UpdateProduct(Product input);


        /// <summary>
        /// Obtiene lista de productos en base a la paginación
        /// </summary>
        /// <returns></returns>
        public Task<List<ProductDTO>> GetProducts(int pageNumber, int pageSize);

        /// <summary>
        /// Valida si existe un producto por su nombre
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<bool> ExistsProductByName(string name);

        /// <summary>
        /// Valida si existe un producto por id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public Task<bool> ExistsProductById(int productId);

        /// <summary>
        /// Obtiene un producto en base a su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<ProductDTO> GetProductById(int productId);
               
    }
}