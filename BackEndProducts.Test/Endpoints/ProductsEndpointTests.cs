using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BackEndProducts.Api.Endpoints;
using Microsoft.AspNetCore.Http;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using BackEndProducts.Application.Model;
using AutoMapper;
using Moq;
using BackEndProducts.Application.Commands;
using BackEndProducts.Application.Handlers;
using BackEndProducts.Application.Handlers.InsertProduct;
using BackEndProducts.Application.Interface;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using BackEndProducts.Application.Shared;
using FluentValidation.TestHelper;
using FluentValidation;

namespace BackEndProducts.Api.Endpoints.Tests
{
    public class ProductsEndpointTests
    {
        #region ===[ Private Members ]=============================================================
        private readonly Mock<HttpContext> _context;
        private readonly Mock<ISender> _mediator;
        private readonly Mock<IProductApplication> _productService;
        private readonly Mock<IProductRepository> _productRepository;
        private readonly Mock<IValidator<InsertProductCommand>> _validator;
        private readonly InsertProductHandlerValidationValidator _insertValidator;
        

        #endregion

        #region ===[ Constructor ]=================================================================
        public ProductsEndpointTests()
        {
            _context = new();
            _mediator = new();
            _productService = new();
            _validator = new();
            _insertValidator = new();
            _productRepository = new();
        }


        #endregion
        /// <summary>
        /// Valida la creacion de un producto
        /// </summary>
        [Fact]
        public async Task Handle_Sholuld_Return_FailureResult_When_ProductId_Is_negative_or_Zero()
        {
            //Arrange
            var input = new InputCreateProduct()
            {
                ProductId = -1,
                Name = "test",
                Description = "description",
                Price = 0,
                Stock = 10
            };

            //var item = new Item { OrgUid = 123456, CheckDate = new DateTime(2020, 4, 1) };

            var command = new InsertProductCommand(input);
            var handler = new InsertProductHandler(_productService.Object, _validator.Object);


            //Act
            //Valida el fluentValdation
            TestValidationResult<InsertProductCommand> response = _insertValidator.TestValidate(command);


            //Result<ResultRequestDTO> result = await handler.Handle(command, default);

            //Assert
            //result.Should().NotBeNull();   // Se espera que no  sea NULO
            //result.IsSucess.Should().BeFalse();
            //result.Error.Should().Be(DomainErrors.ProductCreationIdEmpty.message);

            //Assert
            //response.IsValid!.Should().BeTrue();  // debe devolver false
            //response.Errors.Should().HaveCount(1);  // debe tener al menos un error
            //response.Errors[0].Should().Be(DomainErrors.ProductCreationIdInvalid.message);

            //response.ShouldHaveValidationErrorFor(x => x.input.ProductId).Only();  // con "Only" solo sale ese mensaje
            response.ShouldHaveValidationErrorFor(x => x.input.ProductId).WithErrorMessage(DomainErrors.ProductCreationIdEmpty.message);
            response.ShouldNotHaveValidationErrorFor(x => x.input.Name);
            response.ShouldNotHaveValidationErrorFor(x => x.input.Stock);
            response.ShouldNotHaveValidationErrorFor(x => x.input.Description);

        }

        [Fact]
        public async Task Handle_Sholuld_Return_FailureResult_When_Name_Is_greatest()
        {
            //Arrange
            var input = new InputCreateProduct()
            {
                ProductId = 2,
                Name = "descriptiondescriptiondescriptiondescriptiondescriptiondescription",
                Description = "",
                Price = 0,
                Stock = 10
            };

            //var item = new Item { OrgUid = 123456, CheckDate = new DateTime(2020, 4, 1) };

            var command = new InsertProductCommand(input);
            var handler = new InsertProductHandler(_productService.Object, _validator.Object);


            //Act
            //Valida el fluentValdation
            TestValidationResult<InsertProductCommand> response = _insertValidator.TestValidate(command);


            //Result<ResultRequestDTO> result = await handler.Handle(command, default);

            //Assert
            //result.Should().NotBeNull();   // Se espera que no  sea NULO
            //result.IsSucess.Should().BeFalse();
            //result.Error.Should().Be(DomainErrors.ProductCreationIdEmpty.message);

            //Assert
            //response.IsValid!.Should().BeTrue();  // debe devolver false
            //response.Errors.Should().HaveCount(1);  // debe tener al menos un error
            //response.Errors[0].Should().Be(DomainErrors.ProductCreationIdInvalid.message);

            //Assert
            //response.ShouldHaveValidationErrorFor(x => x.input.ProductId).Only();  // con "Only" solo sale ese mensaje
            response.ShouldNotHaveValidationErrorFor(x => x.input.ProductId);
            response.ShouldHaveValidationErrorFor(x => x.input.Name).WithErrorMessage(DomainErrors.ProductCreationNameInvalid.message);
            response.ShouldNotHaveValidationErrorFor(x => x.input.Stock);
            response.ShouldNotHaveValidationErrorFor(x => x.input.Description);

        }

        [Fact]
        public async Task Handle_Sholuld_Return_FailureResult_When_Id_Is_Duplicate()
        {
            //Arrange
            var input = new InputCreateProduct()
            {
                ProductId = 2,
                Name = "aaaaaaaa",
                Description = "",
                Price = 10,
                Stock = 10
            };
                   
            //var item = new Item { OrgUid = 123456, CheckDate = new DateTime(2020, 4, 1) };

            var command = new InsertProductCommand(input);
            var handler = new InsertProductHandler(_productService.Object, _validator.Object);


            //Act
            //Valida el fluentValdation
            TestValidationResult<InsertProductCommand> response = _insertValidator.TestValidate(command);
            Result<ResultRequestDTO> result = await handler.Handle(command, default);

            //Assert
   

            //Assert
            //response.IsValid!.Should().BeTrue();  // debe devolver false
            //response.Errors.Should().HaveCount(1);  // debe tener al menos un error
            //response.Errors[0].Should().Be(DomainErrors.ProductCreationIdInvalid.message);

            //Assert
            //response.ShouldHaveValidationErrorFor(x => x.input.ProductId).Only();  // con "Only" solo sale ese mensaje
            response.ShouldNotHaveValidationErrorFor(x => x.input.ProductId);
            response.ShouldNotHaveValidationErrorFor(x => x.input.Name);
            response.ShouldNotHaveValidationErrorFor(x => x.input.Stock);
            response.ShouldNotHaveValidationErrorFor(x => x.input.Description);

            result.Should().NotBeNull();   // Se espera que no  sea NULO
            result.IsSucess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.ProductCreationIdDuplicated.message);

        }
    }
}