using BackEndProducts.Application.Commands;
using BackEndProducts.Application.Interface;
using BackEndProducts.Application.Model;
using BackEndProducts.Application.CQRS;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using FluentValidation.Results;
using BackEndProducts.Application.Shared;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using AutoMapper;
using BackEndProducts.Common;
using static BackEndProducts.Common.Enum;

namespace BackEndProducts.Application.Handlers.InsertProduct
{

    public class InsertProductHandlerValidationValidator : AbstractValidator<InsertProductCommand>
    {
        public InsertProductHandlerValidationValidator()
        {
            RuleFor(x => x.input.ProductId)
                        .NotEmpty()
                        .GreaterThan(0)
                        .WithMessage(DomainErrors.ProductCreationIdEmpty.message); // lanza error si es nulo o vacio el valor

            RuleFor(x => x.input.Name).NotEmpty().WithMessage(DomainErrors.ProductCreationNameIsEmpty.message);  // lanza error si es nulo o vacio el valor
            RuleFor(x => x.input.Name).MaximumLength(50).WithMessage(DomainErrors.ProductCreationNameInvalid.message);  // lanza error si la cantidad de caracteres es mayor al valor especificado

            //RuleFor(x => x.input.Status)                        
            //            .InclusiveBetween(0, 1)
            //            .WithMessage("Valor no válido. Valor debe ser 0 o 1"); // lanza error si el valor está en ese rango

            RuleFor(x => x.input.Stock)
                        .LessThan(Int32.MaxValue).WithMessage(DomainErrors.ProductCreationInvalidStock.message); // lanza error si la cantidad de caracteres es mayor al valor especificado

            RuleFor(x => x.input.Price)
                        .GreaterThan(0)
                        .WithMessage(DomainErrors.ProductCreationPriceInvalid.message); // lanza error si el valor es menor o igual al valor especificado          

            RuleFor(x => x.input.Description).MaximumLength(100).WithMessage(DomainErrors.ProductCreationDescriptionInvalid.message); // lanza error si el valor es menor o igual al valor especificado                                    

        }
    }

    public class InsertProductHandler : ICommandHandler<InsertProductCommand, Result<ResultRequestDTO>>
    {
        private readonly IProductApplication _ProductService;
        private readonly IValidator<InsertProductCommand> _validator;

        public InsertProductHandler(IProductApplication ProductApplication, IValidator<InsertProductCommand> validator)
        {
            _ProductService = ProductApplication;
            _validator = validator;
        }

        public async Task<Result<ResultRequestDTO>> Handle(InsertProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                ServiceLog.Write(LogType.WebSite, System.Diagnostics.TraceLevel.Info, nameof(Handle), "Inicio");

                _validator.ValidateAndThrow(command);

                Result<ResultRequestDTO> outPut = await _ProductService.InsertProduct(command.input);

                ServiceLog.Write(LogType.WebSite, System.Diagnostics.TraceLevel.Info, nameof(Handle), $" IsSucess: {outPut?.IsSucess}  msg: {outPut?.Error}");
            
                return outPut;
        }
            catch (Exception ex)
            {
                ServiceLog.Write(LogType.WebSite, ex, nameof(Handle), "error");
                throw;
            }
}
    }
}

