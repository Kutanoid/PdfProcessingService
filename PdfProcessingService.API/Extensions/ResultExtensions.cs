namespace PdfProcessingService.API.Extensions
{
    public static class ResultExtensions
    {
        public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> bind)
        {
            return result.IsSuccess ?
                bind(result.Value) :
                Result<TOut>.Failure(result.Error);
        }

        public static Result<TOut> TryCatch<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> func, Error error) 
        {
            try
            {
                return result.IsSuccess ?
                Result<TOut>.Success(func(result.Value)) :
                Result<TOut>.Failure(result.Error);
            }
            catch
            {
                return Result<TOut>.Failure(error);
            }
        }

        public static Result<TIn> RunProcedure<TIn>(this Result<TIn> result, Action<TIn> procedure)
        {
            return result.IsSuccess ?
                Result<TIn>.Success(result.Value) :
                Result<TIn>.Failure(result.Error);
        }

        public static TOut ProduceResponse<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> success, Func<Error, TOut> failure)
        {
            return result.IsSuccess ?
                success(result.Value) :
                failure(result.Error);
        }
    }
}
