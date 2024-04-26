using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Interfaces
{
    public interface IProcessHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : class, IRequest<TResponse>
    {
    }
}
