using PdfProcessingService.API;
using PdfProcessingService.API.Extensions;
using PdfProcessingService.API.Models;
using PdfProcessingService.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions();

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer();
//builder.Services.ConfigureOptions<JwtBearerOptions>();
//builder.Services.ConfigureOptions<JwtOptions>();
//builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("api/pdf/extrect-attachment", (ExtractAttachmentModel model) =>
{
    return DocumentProcessingService.ValidateRequest(model)
    .Bind(_ => DocumentProcessingService.ValidateRequestProperties(model))
    .TryCatch(_ => DocumentProcessingService.LoadPdfDocument(model.DocumentData), Error.UnableToLoadDataAsPdf)
    .ProduceResponse(
        input => Results.Ok(DocumentProcessingService.ExtractAttachment(input, model.AttachmentDescription)),
        error => DocumentProcessingService.ProduceErrorResponse(error));
});

app.MapPost("api/pdf/embed-attachment", (EmbedAttachmentModel model) =>
{
    return DocumentProcessingService.ValidateRequest(model)
    .Bind(_ => DocumentProcessingService.ValidateRequestProperties(model))
    .TryCatch(_ => DocumentProcessingService.LoadPdfDocument(model.DocumentData), Error.UnableToLoadDataAsPdf)
    .TryCatch(document => DocumentProcessingService.TransformDocument(document), Error.UnableToTransformDocument)
    .TryCatch(document => DocumentProcessingService.AddDocumentToPdf(document), Error.UnableToAddDocumentToPdf)
    .RunProcedure(document => DocumentProcessingService.AddAttachment(document, model.AttachmentData, model.AttachmentName))
    .ProduceResponse(
        input => Results.Ok(DocumentProcessingService.TransformDocument(input)),
        error => DocumentProcessingService.ProduceErrorResponse(error));
});

app.Run();