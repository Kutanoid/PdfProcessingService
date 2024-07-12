namespace PdfProcessingService.API.Extensions
{
    public static class ResultsExtensions
    {
        public static IResult InternalServerError(this IResultExtensions extensions, string message)
        {
            return new InternalServerErrorResult(message);
        }
    }

    public class InternalServerErrorResult : IResult
    {
        private readonly string _message;

        public InternalServerErrorResult(string message)
        {
            _message = message;  
        }

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 500;
            await httpContext.Response.WriteAsJsonAsync(_message);
        }
    }
}
