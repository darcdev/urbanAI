# Ejemplo de Implementación: Sistema de Gestión de Documentos de Usuario

Este documento muestra un ejemplo completo de cómo implementar un sistema de gestión de documentos utilizando el nuevo servicio de MinIO.

## Estructura de Carpetas

```
Urban.AI.Application/
├── Documents/
│   ├── Dtos/
│   │   ├── DocumentRequest.cs
│   │   ├── DocumentResponse.cs
│   │   └── DocumentUrlResponse.cs
│   ├── UploadDocument/
│   │   ├── UploadDocumentCommand.cs
│   │   ├── UploadDocumentHandler.cs
│   │   └── UploadDocumentValidation.cs
│   ├── GetDocument/
│   │   ├── GetDocumentQuery.cs
│   │   └── GetDocumentHandler.cs
│   ├── GetDocumentUrl/
│   │   ├── GetDocumentUrlQuery.cs
│   │   └── GetDocumentUrlHandler.cs
│   ├── DeleteDocument/
│   │   ├── DeleteDocumentCommand.cs
│   │   └── DeleteDocumentHandler.cs
│   └── DocumentExtensions.cs
```

## 1. Domain Layer

### Document.cs
```csharp
namespace Urban.AI.Domain.Documents;

public sealed class Document : Entity
{
    #region Constants
    private const int FileNameMaxLength = 255;
    private const int PathMaxLength = 500;
    private const int BucketNameMaxLength = 63;
    private const int MimeTypeMaxLength = 100;
    #endregion

    public Guid UserId { get; private set; }
    public string FileName { get; private set; }
    public string FilePath { get; private set; }
    public string BucketName { get; private set; }
    public string MimeType { get; private set; }
    public long FileSizeInBytes { get; private set; }
    public DateTime UploadedAt { get; private set; }

    private Document() { }

    public static Document Create(
        Guid userId,
        string fileName,
        string filePath,
        string bucketName,
        string mimeType,
        long fileSizeInBytes)
    {
        return new Document
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FileName = fileName,
            FilePath = filePath,
            BucketName = bucketName,
            MimeType = mimeType,
            FileSizeInBytes = fileSizeInBytes,
            UploadedAt = DateTime.UtcNow
        };
    }

    public void UpdateMetadata(string fileName, string mimeType)
    {
        FileName = fileName;
        MimeType = mimeType;
    }
}
```

### IDocumentRepository.cs
```csharp
namespace Urban.AI.Domain.Documents;

#region Usings
using Urban.AI.Domain.Common.Abstractions;
#endregion

public interface IDocumentRepository : IRepository<Document>
{
    Task<IEnumerable<Document>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Document?> GetByFilePathAsync(
        string filePath,
        string bucketName,
        CancellationToken cancellationToken = default);
}
```

### DocumentErrors.cs
```csharp
namespace Urban.AI.Domain.Documents;

#region Usings
using Urban.AI.Domain.Common;
#endregion

public static class DocumentErrors
{
    public static readonly Error NotFound = new(
        "Document.NotFound",
        "The document was not found");

    public static readonly Error UnauthorizedAccess = new(
        "Document.UnauthorizedAccess",
        "You do not have permission to access this document");

    public static readonly Error UploadFailed = new(
        "Document.UploadFailed",
        "Failed to upload the document");

    public static readonly Error InvalidFileFormat = new(
        "Document.InvalidFileFormat",
        "The file format is not supported");

    public static Error FileTooLarge(long maxSizeInMb) => new(
        "Document.FileTooLarge",
        $"The file size exceeds the maximum allowed size of {maxSizeInMb} MB");
}
```

## 2. Infrastructure Layer

### DocumentRepository.cs
```csharp
namespace Urban.AI.Infrastructure.Database.Repositories;

#region Usings
using Urban.AI.Domain.Documents;
using Urban.AI.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
#endregion

internal sealed class DocumentRepository : Repository<Document>, IDocumentRepository
{
    public DocumentRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Document>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<Document>()
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Document?> GetByFilePathAsync(
        string filePath,
        string bucketName,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<Document>()
            .FirstOrDefaultAsync(
                d => d.FilePath == filePath && d.BucketName == bucketName,
                cancellationToken);
    }
}
```

### DocumentMapping.cs
```csharp
namespace Urban.AI.Infrastructure.Database.Mappings;

#region Usings
using Urban.AI.Domain.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#endregion

internal sealed class DocumentMapping : IEntityTypeConfiguration<Document>
{
    #region Constants
    private const string TableName = "Documents";
    private const int FileNameMaxLength = 255;
    private const int PathMaxLength = 500;
    private const int BucketNameMaxLength = 63;
    private const int MimeTypeMaxLength = 100;
    #endregion

    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(d => d.Id);

        builder.Property(d => d.UserId)
            .IsRequired();

        builder.Property(d => d.FileName)
            .HasMaxLength(FileNameMaxLength)
            .IsRequired();

        builder.Property(d => d.FilePath)
            .HasMaxLength(PathMaxLength)
            .IsRequired();

        builder.Property(d => d.BucketName)
            .HasMaxLength(BucketNameMaxLength)
            .IsRequired();

        builder.Property(d => d.MimeType)
            .HasMaxLength(MimeTypeMaxLength)
            .IsRequired();

        builder.Property(d => d.FileSizeInBytes)
            .IsRequired();

        builder.Property(d => d.UploadedAt)
            .IsRequired();

        builder.HasIndex(d => d.UserId);
        builder.HasIndex(d => new { d.FilePath, d.BucketName })
            .IsUnique();
    }
}
```

## 3. Application Layer

### DTOs

#### DocumentRequest.cs
```csharp
namespace Urban.AI.Application.Documents.Dtos;

public sealed record DocumentRequest(
    string FileName,
    string Base64Content,
    string MimeType,
    string? Category = null);
```

#### DocumentResponse.cs
```csharp
namespace Urban.AI.Application.Documents.Dtos;

public sealed record DocumentResponse(
    Guid Id,
    string FileName,
    string MimeType,
    long FileSizeInBytes,
    DateTime UploadedAt,
    string PresignedUrl);
```

#### DocumentUrlResponse.cs
```csharp
namespace Urban.AI.Application.Documents.Dtos;

public sealed record DocumentUrlResponse(
    string Url,
    int ExpiresInHours);
```

### Upload Document

#### UploadDocumentCommand.cs
```csharp
namespace Urban.AI.Application.Documents.UploadDocument;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Documents.Dtos;
#endregion

public sealed record UploadDocumentCommand(
    DocumentRequest Request) : ICommand<Guid>;
```

#### UploadDocumentValidation.cs
```csharp
namespace Urban.AI.Application.Documents.UploadDocument;

#region Usings
using Urban.AI.Application.Documents.Dtos;
using FluentValidation;
#endregion

internal sealed class UploadDocumentValidation : AbstractValidator<UploadDocumentCommand>
{
    #region Constants
    private const int MaxFileSizeInMb = 10;
    private const long MaxFileSizeInBytes = MaxFileSizeInMb * 1024 * 1024;
    private static readonly string[] AllowedMimeTypes = 
    {
        "application/pdf",
        "image/jpeg",
        "image/png",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    };
    #endregion

    public UploadDocumentValidation()
    {
        RuleFor(x => x.Request.FileName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Request.Base64Content)
            .NotEmpty()
            .Must(BeValidBase64)
            .WithMessage("The file content must be a valid Base64 string")
            .Must(content => GetBase64Size(content) <= MaxFileSizeInBytes)
            .WithMessage($"The file size must not exceed {MaxFileSizeInMb} MB");

        RuleFor(x => x.Request.MimeType)
            .NotEmpty()
            .Must(type => AllowedMimeTypes.Contains(type))
            .WithMessage($"The file type must be one of: {string.Join(", ", AllowedMimeTypes)}");
    }

    private static bool BeValidBase64(string base64String)
    {
        if (string.IsNullOrWhiteSpace(base64String))
            return false;

        var content = base64String.Contains("base64,")
            ? base64String.Split("base64,")[1]
            : base64String;

        try
        {
            Convert.FromBase64String(content);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static long GetBase64Size(string base64String)
    {
        var content = base64String.Contains("base64,")
            ? base64String.Split("base64,")[1]
            : base64String;

        var padding = content.EndsWith("==") ? 2 : content.EndsWith("=") ? 1 : 0;
        return (content.Length * 3) / 4 - padding;
    }
}
```

#### UploadDocumentHandler.cs
```csharp
namespace Urban.AI.Application.Documents.UploadDocument;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Common.Abstractions.Data;
using Urban.AI.Application.Common.Abstractions.Storage;
using Urban.AI.Domain.Common;
using Urban.AI.Domain.Common.File;
using Urban.AI.Domain.Documents;
#endregion

internal sealed class UploadDocumentHandler : ICommandHandler<UploadDocumentCommand, Guid>
{
    #region Constants
    private const string DocumentsBucketName = "user-documents";
    #endregion

    #region Private Members
    private readonly IDocumentRepository _documentRepository;
    private readonly IStorageService _storageService;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    #endregion

    public UploadDocumentHandler(
        IDocumentRepository documentRepository,
        IStorageService storageService,
        IUserContext userContext,
        IUnitOfWork unitOfWork)
    {
        _documentRepository = documentRepository;
        _storageService = storageService;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        UploadDocumentCommand command,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;
        var request = command.Request;

        var filePath = BuildFilePath(userId, request.Category);

        var file = File.CreateForSave(
            filename: request.FileName,
            content: request.Base64Content,
            path: filePath,
            nameBucket: DocumentsBucketName,
            mimetype: request.MimeType);

        var objectKey = await _storageService.SaveFile(file, cancellationToken);

        var fileSizeInBytes = CalculateFileSize(request.Base64Content);

        var document = Document.Create(
            userId: userId,
            fileName: request.FileName,
            filePath: objectKey,
            bucketName: DocumentsBucketName,
            mimeType: request.MimeType,
            fileSizeInBytes: fileSizeInBytes);

        _documentRepository.Add(document);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(document.Id);
    }

    private static string BuildFilePath(Guid userId, string? category)
    {
        var year = DateTime.UtcNow.Year;
        var month = DateTime.UtcNow.Month;

        return string.IsNullOrWhiteSpace(category)
            ? $"users/{userId}/{year}/{month:D2}"
            : $"users/{userId}/{category}/{year}/{month:D2}";
    }

    private static long CalculateFileSize(string base64Content)
    {
        var content = base64Content.Contains("base64,")
            ? base64Content.Split("base64,")[1]
            : base64Content;

        var padding = content.EndsWith("==") ? 2 : content.EndsWith("=") ? 1 : 0;
        return (content.Length * 3) / 4 - padding;
    }
}
```

### Get Document URL

#### GetDocumentUrlQuery.cs
```csharp
namespace Urban.AI.Application.Documents.GetDocumentUrl;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
#endregion

public sealed record GetDocumentUrlQuery(
    Guid DocumentId,
    int ExpiryInHours = 24) : IQuery<string>;
```

#### GetDocumentUrlHandler.cs
```csharp
namespace Urban.AI.Application.Documents.GetDocumentUrl;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Common.Abstractions.Storage;
using Urban.AI.Domain.Common;
using Urban.AI.Domain.Common.File;
using Urban.AI.Domain.Documents;
#endregion

internal sealed class GetDocumentUrlHandler : IQueryHandler<GetDocumentUrlQuery, string>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IStorageService _storageService;
    private readonly IUserContext _userContext;

    public GetDocumentUrlHandler(
        IDocumentRepository documentRepository,
        IStorageService storageService,
        IUserContext userContext)
    {
        _documentRepository = documentRepository;
        _storageService = storageService;
        _userContext = userContext;
    }

    public async Task<Result<string>> Handle(
        GetDocumentUrlQuery query,
        CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(
            query.DocumentId,
            cancellationToken);

        if (document is null)
        {
            return Result.Failure<string>(DocumentErrors.NotFound);
        }

        if (document.UserId != _userContext.UserId)
        {
            return Result.Failure<string>(DocumentErrors.UnauthorizedAccess);
        }

        var file = File.CreateForGet(
            filename: document.FileName,
            path: document.FilePath,
            nameBucket: document.BucketName);

        var presignedUrl = await _storageService.GetPresignedUrl(
            file,
            query.ExpiryInHours,
            cancellationToken);

        return Result.Success(presignedUrl);
    }
}
```

### Delete Document

#### DeleteDocumentCommand.cs
```csharp
namespace Urban.AI.Application.Documents.DeleteDocument;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
#endregion

public sealed record DeleteDocumentCommand(Guid DocumentId) : ICommand;
```

#### DeleteDocumentHandler.cs
```csharp
namespace Urban.AI.Application.Documents.DeleteDocument;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Common.Abstractions.Data;
using Urban.AI.Application.Common.Abstractions.Storage;
using Urban.AI.Domain.Common;
using Urban.AI.Domain.Common.File;
using Urban.AI.Domain.Documents;
#endregion

internal sealed class DeleteDocumentHandler : ICommandHandler<DeleteDocumentCommand>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IStorageService _storageService;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDocumentHandler(
        IDocumentRepository documentRepository,
        IStorageService storageService,
        IUserContext userContext,
        IUnitOfWork unitOfWork)
    {
        _documentRepository = documentRepository;
        _storageService = storageService;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteDocumentCommand command,
        CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(
            command.DocumentId,
            cancellationToken);

        if (document is null)
        {
            return Result.Failure(DocumentErrors.NotFound);
        }

        if (document.UserId != _userContext.UserId)
        {
            return Result.Failure(DocumentErrors.UnauthorizedAccess);
        }

        var file = File.CreateForDelete(
            path: document.FilePath,
            nameBucket: document.BucketName);

        await _storageService.DeleteFile(file, cancellationToken);

        _documentRepository.Remove(document);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
```

### DocumentExtensions.cs
```csharp
namespace Urban.AI.Application.Documents;

#region Usings
using Urban.AI.Application.Documents.Dtos;
using Urban.AI.Domain.Documents;
#endregion

internal static class DocumentExtensions
{
    public static DocumentResponse ToResponse(this Document document, string presignedUrl)
    {
        return new DocumentResponse(
            document.Id,
            document.FileName,
            document.MimeType,
            document.FileSizeInBytes,
            document.UploadedAt,
            presignedUrl);
    }

    public static async Task<DocumentResponse> ToResponseWithUrlAsync(
        this Document document,
        Func<Task<string>> getPresignedUrl)
    {
        var presignedUrl = await getPresignedUrl();
        return document.ToResponse(presignedUrl);
    }
}
```

## 4. Web API Layer

### DocumentsController.cs
```csharp
namespace Urban.AI.WebApi.Controllers;

#region Usings
using Urban.AI.Application.Documents.DeleteDocument;
using Urban.AI.Application.Documents.Dtos;
using Urban.AI.Application.Documents.GetDocumentUrl;
using Urban.AI.Application.Documents.UploadDocument;
using Urban.AI.WebApi.Configurations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#endregion

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/documents")]
public class DocumentsController : ControllerBase
{
    private readonly ISender _sender;

    public DocumentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadDocument(
        [FromBody] DocumentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UploadDocumentCommand(request);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(GetDocumentUrl),
            new { documentId = result.Value },
            result.Value);
    }

    [HttpGet("{documentId:guid}/url")]
    [ProducesResponseType(typeof(DocumentUrlResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDocumentUrl(
        Guid documentId,
        [FromQuery] int expiryHours = 24,
        CancellationToken cancellationToken)
    {
        var query = new GetDocumentUrlQuery(documentId, expiryHours);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(new DocumentUrlResponse(result.Value, expiryHours));
    }

    [HttpDelete("{documentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDocument(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteDocumentCommand(documentId);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return NoContent();
    }
}
```

## 5. Configuración

### appsettings.json
```json
{
  "Minio": {
    "Host": "localhost:9000",
    "Username": "rootadmin",
    "Password": "rootadmin",
    "IsSecureSSL": false,
    "AutoCreateBuckets": true,
    "PresignedUrlExpiryInHours": 24
  }
}
```

## 6. Ejemplo de Uso con Cliente

### Request de Subida
```bash
curl -X POST "http://localhost:5000/api/v1/documents" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "fileName": "contract.pdf",
    "base64Content": "data:application/pdf;base64,JVBERi0xLjQKJeLjz9MKM...",
    "mimeType": "application/pdf",
    "category": "contracts"
  }'
```

### Response
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Obtener URL del Documento
```bash
curl -X GET "http://localhost:5000/api/v1/documents/3fa85f64-5717-4562-b3fc-2c963f66afa6/url?expiryHours=48" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Response
```json
{
  "url": "http://localhost:9000/user-documents/users/123.../contract.pdf?X-Amz-Algorithm=AWS4-HMAC-SHA256&...",
  "expiresInHours": 48
}
```

## Conclusión

Este ejemplo demuestra:

✅ Uso completo del servicio de MinIO con URLs pre-firmadas
✅ Implementación siguiendo Clean Architecture
✅ Validaciones con FluentValidation
✅ Manejo de errores con Result Pattern
✅ Autorización y seguridad por usuario
✅ Organización de archivos por fecha y categoría
✅ Metadata de documentos en base de datos
