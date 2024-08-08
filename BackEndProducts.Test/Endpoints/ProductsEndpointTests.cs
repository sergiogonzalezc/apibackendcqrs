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

namespace BackEndProducts.Api.Endpoints.Tests
{
    public class ProductsEndpointTests
    {
        #region ===[ Private Members ]=============================================================
        private readonly Mock<HttpContext> _context;
        private readonly Mock<ISender> _mediator;
        private readonly Mock<IProductApplication> _productService;

        #endregion

        #region ===[ Constructor ]=================================================================
        public ProductsEndpointTests()
        {
            _context = new();
            _mediator = new();
            _productService = new();
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

            var command = new InsertProductCommand(input);
            var handler = new InsertProductHandler(_productService.Object);


            //Act
            Result<ResultRequestDTO> result = await handler.Handle(command, default);

            //Assert
            result.Should().NotBeNull();   // Se espera que no  sea NULO
            result.IsSucess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.ProductCreationIdInvalid.message);

        }
    }
}