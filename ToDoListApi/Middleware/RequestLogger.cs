using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace TodoListApi.Middleware
{
    public class RequestLogger
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLogger> _logger;

        public RequestLogger(RequestDelegate next, ILogger<RequestLogger> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext ctx)
        {
            try
            {
                await _next(ctx);
            }
            finally
            {
                _logger.LogInformation(
                    $"Request {ctx.Request?.Method} {ctx.Request?.Path.Value} {ctx.Request?.QueryString.Value} => {ctx.Response?.StatusCode}"
                );
            }
        }
    }
}
