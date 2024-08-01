using AutoMapper;
using BackEndProducts.Application.Commands;
using BackEndProducts.Application.Handlers;
using BackEndProducts.Application.Model;
using BackEndProducts.Common;
using BackEndProducts.Api.Model;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FluentValidation;
using BackEndProducts.Application.Common;
using BackEndProducts.Application.Handlers.GetProduct;
using Microsoft.AspNetCore.Builder;
using NLog.Filters;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using System.IO;
using MySqlX.XDevAPI.Common;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using BackEndProducts.Application.Handlers.GetProductById;

namespace BackEndProducts.Api.Endpoints
{
    public class ProductsEndpoint : ICarterModule
    {
        private static Stopwatch _stopWatch = new Stopwatch();
        private static string cacheKey = "esto_es_una_llave_para_cache";

        private static Dictionary<int, string> estadosProducto = new Dictionary<int, string>();

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/products").WithOpenApi();

            group.MapPost("", InsertProduct)
                        .Produces<InsertProductModel>(StatusCodes.Status201Created)
                        .ProducesProblem(StatusCodes.Status400BadRequest)
                        .WithSummary("Inserta un nuevo producto")
                        .WithDescription("Este endpoint inserta un nuevo producto en BD SQL")
                        .AddEndpointFilter<EndpointExecutionFilter>();

            group.MapGet("", GetProducts)
                        .Produces<ProductModel>(StatusCodes.Status200OK)
                        .ProducesProblem(StatusCodes.Status400BadRequest)
                        .ProducesProblem(StatusCodes.Status404NotFound)
                        .WithSummary("Obtiene lista de productos")
                        .WithDescription("Obtiene lista de productos paginando de 10 en 10 por defecto")
                        .AddEndpointFilter<EndpointExecutionFilter>();

            group.MapGet("{id}", GetProductsById).WithName(nameof(GetProductsById))
                        .Produces<ProductModel>(StatusCodes.Status200OK)
                        .ProducesProblem(StatusCodes.Status400BadRequest)
                        .ProducesProblem(StatusCodes.Status404NotFound)
                        .WithSummary("Obtiene un producto en particular por su ID único")
                        .WithDescription("Obtiene un producto por su ID")
                        .AddEndpointFilter<EndpointExecutionFilter>();


            group.MapPut("{id}", UpdateProduct).WithName(nameof(UpdateProduct))
                        .Produces<UpdateProductModel>(StatusCodes.Status200OK)
                        .ProducesProblem(StatusCodes.Status400BadRequest)
                        .ProducesProblem(StatusCodes.Status404NotFound)
                        .WithSummary("Actualiza un producto por su ID")
                        .WithDescription("Actualiza un producto en base a su ID")
                        .AddEndpointFilter<EndpointExecutionFilter>();
        }


        /// <summary>
        /// Obtiene lista de productos paginados
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<List<ProductDTO>> GetProducts(HttpContext context, ISender mediator, [AsParameters] GetProductsWithPaginationQuery request)
        {
            _stopWatch.Start();

            // Implement a CQRS for query/command responsibility segregation
            List<ProductDTO> output = await mediator.Send(request);

            _stopWatch.Stop();

            string host = context?.Request?.Host.Value;
            string path = context?.Request?.Path.Value;

            ServiceLog.Write(Common.Enum.LogType.WebSite, TraceLevel.Info, $"TIME_REGISTRY {nameof(GetProducts)}", $"host: {host} path {path} Tiempo en segundos: [{_stopWatch.Elapsed.TotalSeconds}]");

            return output;
        }

        /// <summary>
        /// Obtiene un producto por su ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mediator"></param>
        /// <returns></returns>
        public static async Task<ProductDTO> GetProductsById(IMemoryCache memoryCache, HttpContext context, int id, ISender mediator)
        {
            _stopWatch.Start();

            // Implement a CQRS for query/command responsibility segregation
            var query = new GetProductsByIdQuerys(id);
            _stopWatch.Stop();

            string host = context?.Request?.Host.Value;
            string path = context?.Request?.Path.Value;

            ServiceLog.Write(Common.Enum.LogType.WebSite, TraceLevel.Info, $"TIME_REGISTRY {nameof(GetProductsById)}", $"host: {host} path {path} Tiempo en segundos: [{_stopWatch.Elapsed.TotalSeconds}]");

            return await mediator.Send(query);
        }


        /// <summary>
        /// Inserta un nuevo producto
        /// </summary>
        /// <param name="input"></param>
        /// <param name="mediator"></param>
        /// <returns></returns>
        public static async Task<ResultRequestDTO> InsertProduct(HttpContext context, [FromBody] InputCreateProduct input, ISender mediator)
        {
            string nameMethod = nameof(InsertProduct);

            _stopWatch.Start();

            // Implement a CQRS for query/command responsibility segregation

            var query = new InsertProductCommand(input);
            ResultRequestDTO result = await mediator.Send(query);

            _stopWatch.Stop();
            string host = context?.Request?.Host.Value;
            string path = context?.Request?.Path.Value;

            ServiceLog.Write(Common.Enum.LogType.WebSite, TraceLevel.Info, $"TIME_REGISTRY {nameof(InsertProduct)}", $"host: {host} path {path} Tiempo en segundos: [{_stopWatch.Elapsed.TotalSeconds}]");

            return result;
        }


        /// <summary>
        /// Actualiza un producto
        /// </summary>
        /// <param name="input"></param>
        /// <param name="mediator"></param>
        /// <returns></returns>
        public static async Task<ResultRequestDTO> UpdateProduct(HttpContext context, [FromBody] InputUpdateProduct input, ISender mediator)
        {
            string nameMethod = nameof(UpdateProduct);

            _stopWatch.Start();

            // Implement a CQRS for query/command responsibility segregation
            var query = new UpdateProductCommand(input);
            ResultRequestDTO result = await mediator.Send(query);

            _stopWatch.Stop();
            string host = context?.Request?.Host.Value;
            string path = context?.Request?.Path.Value;

            ServiceLog.Write(Common.Enum.LogType.WebSite, TraceLevel.Info, $"TIME_REGISTRY {nameof(UpdateProduct)}", $"host: {host} path {path} Tiempo en segundos: [{_stopWatch.Elapsed.TotalSeconds}]");

            return result;
        }
    }

}

