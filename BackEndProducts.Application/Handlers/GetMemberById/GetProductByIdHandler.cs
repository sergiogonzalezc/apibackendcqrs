using BackEndProducts.Application.Commands;
using BackEndProducts.Application.Interface;
using BackEndProducts.Application.Model;
using BackEndProducts.Application.Querys;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndProducts.Application.Handlers.GetProductById
{

    public class GetProductByIdHandlerValidationValidator : AbstractValidator<GetProductsByIdQuerys>
    {
        public GetProductByIdHandlerValidationValidator()
        {
            RuleFor(x => x.id)
                        .NotEmpty()
                        .GreaterThan(0)
                        .WithMessage("Valor no válido. Vuelva a intentar ingresando un valor mayor que cero"); // lanza error si es nulo o vacio el valor          
        }
    }

    /// <summary>
    /// Implementa CQRS handler 
    /// </summary>
    public class GetProductByIdHandler : IRequestHandler<GetProductsByIdQuerys, ProductDTO>
    {
        private readonly IProductApplication _productService;

        public GetProductByIdHandler(IProductApplication ProductApplication)
        {
            _productService = ProductApplication;
        }

        public async Task<ProductDTO> Handle(GetProductsByIdQuerys request, CancellationToken cancellationToken)
        {
            return await _productService.GetProductsById(request.id);
        }
    }
}
