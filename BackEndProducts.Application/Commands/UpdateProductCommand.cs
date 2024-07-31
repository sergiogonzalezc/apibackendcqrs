using BackEndProducts.Application.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackEndProducts.Application.CQRS;

namespace BackEndProducts.Application.Commands
{
    public record UpdateProductCommand(InputUpdateProduct input) : ICommand<ResultRequestDTO>
    {
    }
}
