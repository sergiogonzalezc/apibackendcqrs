﻿using BackEndProducts.Application.Commands;
using BackEndProducts.Application.CQRS;
using BackEndProducts.Application.Interface;
using BackEndProducts.Application.Model;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndProducts.Application.Handlers.UpdateProduct
{
    public class UpdateProductHandlerValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductHandlerValidator()
        {
            RuleFor(x => x.input.ProductId)
                        .NotEmpty()
                        .GreaterThan(0)
                        .WithMessage("Valor no válido. Vuelva a intentar ingresando un valor mayor que cero"); // lanza error si es nulo o vacio el valor

            RuleFor(x => x.input.Name).NotEmpty().WithMessage("Valor requerido!");  // lanza error si es nulo o vacio el valor
            RuleFor(x => x.input.Name).MaximumLength(50).WithMessage("Valor debe ser inferior a 50 caracteres");  // lanza error si la cantidad de caracteres es mayor al valor especificado

            RuleFor(x => x.input.Status)
                        .InclusiveBetween(0, 1)
                        .WithMessage("Valor no válido. Valor debe ser 0 o 1"); // lanza error si el valor está en ese rango

            RuleFor(x => x.input.Stock)
                        .LessThan(Int32.MaxValue).WithMessage("Valor no válido."); // lanza error si la cantidad de caracteres es mayor al valor especificado

            RuleFor(x => x.input.Price)
                        .GreaterThan(0)
                        .WithMessage("Valor no válido. Vuelva a intentar ingresando un valor mayor que cero"); // lanza error si el valor es menor o igual al valor especificado          

            RuleFor(x => x.input.Description).MaximumLength(100).WithMessage("Valor no válido. Valor debe ser inferior a 100 caracteres"); // lanza error si el valor es menor o igual al valor especificado                                    

        }
    }

    public class UpdateProductHandler : ICommandHandler<UpdateProductCommand, ResultRequestDTO>
    {
        private readonly IProductApplication _ProductService;

        public UpdateProductHandler(IProductApplication ProductApplication)
        {
            _ProductService = ProductApplication;
        }

        public async Task<ResultRequestDTO> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            return await _ProductService.UpdateProduct(request.input);
        }
    }
}