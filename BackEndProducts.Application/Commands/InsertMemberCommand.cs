using BackEndProducts.Application.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BackEndProducts.Application.CQRS;
using BackEndProducts.Application.Shared;

namespace BackEndProducts.Application.Commands
{
    public record InsertProductCommand(InputCreateProduct input) : ICommand<Result<ResultRequestDTO>>
    {
    }   
}
