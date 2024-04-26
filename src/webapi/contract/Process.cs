using MediatR;
using Newtonsoft.Json;

namespace webapi.Contract
{
    public class Processor : IProcessor
    {
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        public Processor(IConfiguration configuration, IHttpClientFactory httpClientFactory, IMediator mediator)
        {
            _configuration = configuration;
            _mediator = mediator;
        }

        public async Task<TResult> ExecuteAsync<TResult>(object query)
        {
            try
            {
                var respone = await _mediator.Send(query);

                return (TResult)respone;
            }
            catch (Exception ex)
            {
                return JsonConvert.DeserializeObject<TResult>(ex.Message);
            }

        }
    }
}
