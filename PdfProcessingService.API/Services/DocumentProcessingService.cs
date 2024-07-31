using PdfProcessingService.API.Extensions;
using SkiaSharp;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;

namespace PdfProcessingService.API.Services
{
    public static class DocumentProcessingService
    {
        public static Result<T> ValidateRequest<T>(T requestModel)
        {
            return requestModel != null ? 
                   Result<T>.Success(requestModel) : 
                   Result<T>.Failure(Error.MissingRequestData);
        }

        public static Result<T> ValidateRequestProperties<T>(T requestModel)
        {            
            var properties = requestModel
                .GetType()
                .GetProperties()
                .Where(pi => 
                    pi.PropertyType == typeof(string) && 
                    string.IsNullOrEmpty(Convert.ToString(pi.GetValue(requestModel))))
                .Select(pi =>pi.Name)
                .ToArray();

            return properties.Length == 0 ?
                   Result<T>.Success(requestModel) :
                   Result<T>.Failure(Error.MissingRequestDataPropertyValue(properties));
        }

        public static PdfLoadedDocument LoadPdfDocument(string documentData)
        {
            var data = Convert.FromBase64String(documentData);
            return new PdfLoadedDocument(data);
        }

        public static Result<string> ExtractAttachment(PdfLoadedDocument document, string attachmentName)
        {
            if (document.Attachments != null && 
                document.Attachments.Count > 0)
            {
                foreach (PdfAttachment attachment in document.Attachments)
                {
                    if (attachment.FileName == attachmentName)
                    {
                        return Result<string>.Success(Convert.ToBase64String(attachment.Data));
                    }
                }
            }

            document.Dispose();
            return Result<string>.Failure(Error.DocumentMissingAttachment);
        }     

        public static byte[] TransformDocument(PdfLoadedDocument document)
        {
            document.SubstituteFont += SubstituteFont;
            document.ConvertToPDFA(PdfConformanceLevel.Pdf_A3B);
            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream);
                return stream.ToArray();
            }
        }

        public static PdfDocument AddDocumentToPdf(byte[] document)
        {
            var pdfDocument = new PdfDocument(PdfConformanceLevel.Pdf_A3B);
            var pdfLoadedDocument = new PdfLoadedDocument(document);
            pdfDocument.Append(pdfLoadedDocument);
            pdfDocument.ZugferdConformanceLevel = ZugferdConformanceLevel.Basic;
            pdfDocument.ZugferdVersion = ZugferdVersion.ZugferdVersion2_0;
            return pdfDocument;
        }

        public static string TransformDocument(PdfDocument document)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream);
                document.Close(true);

                return Convert.ToBase64String(stream.ToArray());
            }          
        }

        public static void AddAttachment(PdfDocument document, string attachmentData, string attachmentName)
        {
            var attachmentDataBytes = Convert.FromBase64String(attachmentData);
            using (var attachmentStream = new MemoryStream(attachmentDataBytes))
            {
                var attachment = new PdfAttachment($"{attachmentName}.xml", attachmentStream);
                attachment.Relationship = PdfAttachmentRelationship.Alternative;
                attachment.ModificationDate = DateTime.Now;
                attachment.Description = attachmentName;
                attachment.MimeType = "text/xml";

                document.Attachments.Add(attachment);
            }
        }

        public static IResult ProduceErrorResponse(Error error)
        {
            return error.Type switch
            {
                ErrorType.Validation => Results.BadRequest(error.Message),
                _ => Results.Extensions.InternalServerError(error.Message)
            };
        }

        private static void SubstituteFont(object sender, PdfFontEventArgs font)
        {
            var fontName = font.FontName.Split(',')[0];
            var fontStyle = font.FontStyle;
            var sKFontStyle = SKFontStyle.Normal;

            if (fontStyle != PdfFontStyle.Regular)
            {
                if (fontStyle == PdfFontStyle.Bold)
                {
                    sKFontStyle = SKFontStyle.Bold;
                }
                else if (fontStyle == PdfFontStyle.Italic)
                {
                    sKFontStyle = SKFontStyle.Italic;
                }
                else if (fontStyle == (PdfFontStyle.Italic | PdfFontStyle.Bold))
                {
                    sKFontStyle = SKFontStyle.BoldItalic;
                }
            }

            var typeface = SKTypeface.FromFamilyName(fontName, sKFontStyle);
            var typeFaceStream = typeface.OpenStream();            

            if (typeFaceStream != null && typeFaceStream.Length > 0)
            {
                byte[] fontData = new byte[typeFaceStream.Length - 1];
                typeFaceStream.Read(fontData, typeFaceStream.Length);
                typeFaceStream.Dispose();
                using (var fontStream = new MemoryStream(fontData)) 
                {
                    font.FontStream = fontStream;
                }
            }            
        }
    }
}
