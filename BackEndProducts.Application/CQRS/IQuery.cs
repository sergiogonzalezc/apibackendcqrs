﻿using MediatR;

namespace BackEndProducts.Application.CQRS;
public interface IQuery<out TResponse> : IRequest<TResponse>  
    where TResponse : notnull
{
}
