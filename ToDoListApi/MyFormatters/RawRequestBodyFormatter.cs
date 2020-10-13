using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace TodoListApi.MyFormatters
{
    // https://weblog.west-wind.com/posts/2017/sep/14/accepting-raw-request-body-content-in-aspnet-core-api-controllers
    public class RawRequestBodyFormatter : InputFormatter
    {
        private readonly static int MemoryStreamCapacity = 2048;

        private readonly static MediaType TextPlain = new MediaType("text/plain"); 
        private readonly static MediaType OctetStream = new MediaType("application/octet-stream");
    
        public RawRequestBodyFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/octet-stream"));
        }

        private static MediaType GetMediaType(HttpRequest request)
        {
            if (String.IsNullOrEmpty(request?.ContentType))
                return TextPlain;
            return new MediaType(request.ContentType);
        }

        /// processes text/plain, application/octet-stream, and no content-type
        public override bool CanRead(InputFormatterContext ctx)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));

            var mediaType = GetMediaType(ctx.HttpContext.Request);
            if (mediaType.IsSubsetOf(TextPlain) || mediaType.IsSubsetOf(OctetStream))
                return true;

            return false;
        }

        /// text/plain or no content type => string
        /// application/octet-stream => byte[] (those less than RawRequestBodyFormatter.MemoryStreamCapacity)
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext ctx)
        {
            var request = ctx.HttpContext.Request;
            var mediaType = GetMediaType(ctx.HttpContext.Request);

            if (mediaType.IsSubsetOf(TextPlain))
            {
                using (var reader = new StreamReader(request.Body, mediaType.Encoding ?? Encoding.UTF8))
                    return await InputFormatterResult.SuccessAsync(await reader.ReadToEndAsync());
            }
            else if (mediaType.IsSubsetOf(OctetStream))
            {
                if (request.ContentLength > MemoryStreamCapacity)
                    goto fail;
                using (var ms = new MemoryStream(MemoryStreamCapacity))
                {
                    await request.Body.CopyToAsync(ms);
                    return await InputFormatterResult.SuccessAsync(ms.ToArray());
                }
            }
        fail:
            return await InputFormatterResult.FailureAsync();
        }
    }
}