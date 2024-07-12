namespace PdfProcessingService.API.Validation
{
    public static class ValidationMessages
    {
        public static string MissingRequestData => "Missing request data.";
        public static string MissingProperties(string[] props)
        {
            if (props.Length == 1) return $"Missing value for property '{props[0]}'";
            var message = "Missing values for properties";
            for (int i = 0; i < props.Length; i++) 
            {
                if(i > 0)
                {
                    message += ",";
                }
                message += $" '{props[i]}'";
            }          

            return message += ".";
        }
        public static string UnableToLoadDataAsPdf => "Unable to load data as PDF document";
        public static string DocumentMissingAttachment => "Pdf doesn't contain required attachement.";
        public static string UnableToTransformDocument => "Unable to transform target PDF to A-3 Standard.";
        public static string UnableToAddDocumentToPdf => "Unable to add loaded document to PDF";
    }
}
