using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Microsoft.IO;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace DepsWebApp.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation(await ObtainRequestBody(context.Request));

            var originalBodyStream = context.Response.Body;

            await using var responseBody = _recyclableMemoryStreamManager.GetStream();

            context.Response.Body = responseBody;

            await _next.Invoke(context);

            _logger.LogInformation(await ObtainResponseBody(context.Response));

            await responseBody.CopyToAsync(originalBodyStream);
        }

        private static async Task<string> ObtainRequestBody(HttpRequest request)
        {
            if (request.Body == null)
            {
                return string.Empty;
            }

            request.EnableBuffering();
            var encoding = GetEncodingFromContentType(request.ContentType);

            string bodyStr; using (var reader = new StreamReader(request.Body, encoding, true, 1024, true))
            {
                bodyStr = await reader.ReadToEndAsync().ConfigureAwait(false);
            }
            request.Body.Seek(0, SeekOrigin.Begin);

            return bodyStr;
        }

        private static async Task<string> ObtainResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);

            var encoding = GetEncodingFromContentType(response.ContentType);

            using (var reader = new StreamReader(response.Body, encoding, detectEncodingFromByteOrderMarks: false, bufferSize: 4096, leaveOpen: true))
            {
                var text = await reader.ReadToEndAsync().ConfigureAwait(false); response.Body.Seek(0, SeekOrigin.Begin);
                return text;
            }
        }

        private static Encoding GetEncodingFromContentType(string contentTypeStr)
        {
            if (string.IsNullOrEmpty(contentTypeStr))
            {
                return Encoding.UTF8;
            }

            ContentType contentType;

            try
            {
                contentType = new ContentType(contentTypeStr);
            }
            catch (FormatException)
            {
                return Encoding.UTF8;
            }

            if (string.IsNullOrEmpty(contentType.CharSet))
            {
                return Encoding.UTF8;
            }

            return Encoding.GetEncoding(contentType.CharSet, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
        }
    }
}
