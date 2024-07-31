using BackEndProducts.Application.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEndProducts.Application.Querys
{
    public class GetProductsByIdQuerys : IRequest<ProductDTO>
    {
        public int id { get; }

        public GetProductsByIdQuerys(int id)
        {
            this.id = id;
        }

    }
}
