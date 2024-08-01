using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BackEndProducts.Api.Endpoints;
using BackEndProducts.Application.Handlers.GetProduct;
using Microsoft.AspNetCore.Http;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using BackEndProducts.Application.Model;
using AutoMapper;
using FakeItEasy;

namespace BackEndProducts.Api.Endpoints.Tests
{
    public class ProductsEndpointTests
    {
        #region ===[ Private Members ]=============================================================
        private readonly HttpContext _context;
        private readonly ISender _mediator;
        private readonly GetProductsWithPaginationQuery request;

        #endregion

        #region ===[ Constructor ]=================================================================
        public ProductsEndpointTests()
        {
            _context = A.Fake<HttpContext>();
            _mediator = A.Fake<ISender>();
            request = A.Fake<GetProductsWithPaginationQuery>();
        }


        #endregion

        [Fact]
        public void ShouldGetProductsListOK()
        {
            var products = A.Fake<ICollection<ProductDTO>>();
            var productsList = A.Fake<List<ProductDTO>>();
            
            //var endpoint = ProductsEndpoint.GetProducts()

            var result = ProductsEndpoint.GetProducts(_context, _mediator, request);
            Xunit.Assert.IsType<Ok<List<ProductDTO>>>(result);
        }
    }
}