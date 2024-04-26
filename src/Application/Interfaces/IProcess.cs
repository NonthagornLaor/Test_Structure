using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Interfaces
{
    public interface IProcess<TResult> : IRequest<TResult> where TResult : class
    {

    }
}
