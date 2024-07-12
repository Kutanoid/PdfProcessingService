using PdfProcessingService.API.Validation;

namespace PdfProcessingService.API
{
    public class Result<T>
    {
        public T Value { get; init; }
        public Error Error { get; init; }
        public bool IsSuccess { get; init; }

        private Result(T value)
        {
            Value = value;
            IsSuccess = true;
        }

        public Result(Error error)
        {
            Error = error;
            IsSuccess = false;
        }

        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(Error error) => new(error);
    }

    public record Error(ErrorType Type, string Message)
    {       
        public static Error MissingRequestData => new(ErrorType.Validation, ValidationMessages.MissingRequestData);
        public static Error MissingRequestDataPropertyValue(string[] props) => new(ErrorType.Validation, ValidationMessages.MissingProperties(props));
        public static Error UnableToLoadDataAsPdf => new(ErrorType.Failure, ValidationMessages.UnableToLoadDataAsPdf);
        public static Error DocumentMissingAttachment => new(ErrorType.Validation, ValidationMessages.DocumentMissingAttachment);
        public static Error UnableToTransformDocument => new(ErrorType.Failure, ValidationMessages.UnableToTransformDocument);
        public static Error UnableToAddDocumentToPdf => new(ErrorType.Failure, ValidationMessages.UnableToAddDocumentToPdf);
    }

    public enum ErrorType
    {
        Validation,
        Failure
    }
}