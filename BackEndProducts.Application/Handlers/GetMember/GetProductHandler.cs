using BackEndProducts.Application.Common;
using BackEndProducts.Application.CQRS;
using BackEndProducts.Application.Interface;
using BackEndProducts.Application.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BackEndProducts.Application.Handlers.GetProduct
{
    public record GetProductsWithPaginationQuery : IRequest<List<ProductDTO>>
    {    
        public int? PageNumber { get; init; }
        public int? PageSize { get; init; }
    }


    /// <summary>
    /// Implementa manejador de CQRS
    /// </summary>
    public class GetProductHandler : IRequestHandler<GetProductsWithPaginationQuery, List<ProductDTO>>
    {
        private readonly IProductApplication _productService;

        public GetProductHandler(IProductApplication ProductApplication)
        {
            _productService = ProductApplication;
        }

        /// <summary>
        /// Obtiene la lista completa de miembros
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<ProductDTO>> Handle(GetProductsWithPaginationQuery request, CancellationToken cancellationToken)
        {
            return await _productService.GetProducts(request.PageNumber ?? 1, request.PageSize ?? 10);
        }

    }
}
