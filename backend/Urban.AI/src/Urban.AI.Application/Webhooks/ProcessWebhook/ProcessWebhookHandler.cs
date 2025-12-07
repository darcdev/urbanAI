using Microsoft.Extensions.Logging;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Common.Abstractions.Storage;
using Urban.AI.Application.Common.Abstractions.WhatsApp;
using Urban.AI.Application.Webhooks.Dtos;
using Urban.AI.Domain.Common;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Common.File;
using Urban.AI.Domain.Geography;
using Urban.AI.Domain.Incidents;
using Urban.AI.Domain.Leaders;

namespace Urban.AI.Application.Webhooks.ProcessWebhook;

internal sealed class ProcessWebhookHandler : ICommandHandler<ProcessWebhookCommand, ProcessWebhookResponse>
{
    #region Constants
    private const string IncidentImagesBucket = "incident-images";
    private const double DefaultLatitude = 4.6097; // Bogotá por defecto
    private const double DefaultLongitude = -74.0817;
    #endregion

    #region Private Members
    private readonly IWhatsAppMediaService whatsAppMediaService;
    private readonly IWhatsAppMessagingService whatsAppMessagingService;
    private readonly IIncidentAnalysisService incidentAnalysisService;
    private readonly IStorageService storageService;
    private readonly IIncidentRepository incidentRepository;
    private readonly ILeaderRepository leaderRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<ProcessWebhookHandler> logger;
    private double? pendingLatitude;
    private double? pendingLongitude;
    #endregion

    public ProcessWebhookHandler(
        IWhatsAppMediaService whatsAppMediaService,
        IWhatsAppMessagingService whatsAppMessagingService,
        IIncidentAnalysisService incidentAnalysisService,
        IStorageService storageService,
        IIncidentRepository incidentRepository,
        ILeaderRepository leaderRepository,
        IUnitOfWork unitOfWork,
        ILogger<ProcessWebhookHandler> logger)
    {
        this.whatsAppMediaService = whatsAppMediaService;
        this.whatsAppMessagingService = whatsAppMessagingService;
        this.incidentAnalysisService = incidentAnalysisService;
        this.storageService = storageService;
        this.incidentRepository = incidentRepository;
        this.leaderRepository = leaderRepository;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<Result<ProcessWebhookResponse>> Handle(
        ProcessWebhookCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        var processedMessageIds = new List<string>();

        logger.LogInformation(
            "Processing webhook. EventType: {EventType}, IdempotencyKey: {IdempotencyKey}",
            request.EventType,
            request.IdempotencyKey);

        var message = request.Payload.Message;

        if (string.IsNullOrEmpty(message?.Id))
        {
            logger.LogWarning("No message found in webhook payload");
            return Result.Success(processedMessageIds.ToSuccessResponse());
        }

        logger.LogInformation(
            "Processing message. MessageId: {MessageId}, Type: {Type}, From: {From}",
            message.Id,
            message.Type,
            message.From);

        await ProcessMessageAsync(message, request.EventType, request.IdempotencyKey, cancellationToken);
        processedMessageIds.Add(message.Id);

        logger.LogInformation(
            "Webhook processed successfully. ProcessedMessages: {Count}, MessageIds: {MessageIds}",
            processedMessageIds.Count,
            string.Join(", ", processedMessageIds));

        return Result.Success(processedMessageIds.ToSuccessResponse());
    }

    private async Task ProcessMessageAsync(
        KapsoMessage message,
        string eventType,
        string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Message details. MessageId: {MessageId}, Type: {Type}, From: {From}",
            message.Id,
            message.Type,
            message.From);

        if (message.Type == "text" && message.Text != null)
        {
            logger.LogInformation(
                "Text message received. MessageId: {MessageId}, Content: {Content}",
                message.Id,
                message.Text.Body);
        }

        if (message.Type == "location" && message.Location != null)
        {
            pendingLatitude = message.Location.Latitude;
            pendingLongitude = message.Location.Longitude;

            logger.LogInformation(
                "Location received. MessageId: {MessageId}, Latitude: {Latitude}, Longitude: {Longitude}",
                message.Id,
                pendingLatitude,
                pendingLongitude);
        }

        if (message.Type == "image" && message.Image != null)
        {
            await ProcessImageAndCreateIncidentAsync(
                message,
                cancellationToken);
        }
    }

    private async Task ProcessImageAndCreateIncidentAsync(
        KapsoMessage message,
        CancellationToken cancellationToken)
    {
        if (message.Image is null) return;

        try
        {
            logger.LogInformation(
                "Processing image message. MessageId: {MessageId}, ImageId: {ImageId}, ImageUrl: {ImageUrl}, ImageLink: {ImageLink}",
                message.Id,
                message.Image.Id,
                message.Image.Url,
                message.Image.Link);

            var imageUrl = !string.IsNullOrWhiteSpace(message.Image.Link) 
                ? message.Image.Link 
                : !string.IsNullOrWhiteSpace(message.Image.Url) 
                    ? message.Image.Url 
                    : null;

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                logger.LogError(
                    "No image URL provided. MessageId: {MessageId}, ImageId: {ImageId}, ImageUrl: {ImageUrl}, ImageLink: {ImageLink}",
                    message.Id,
                    message.Image.Id,
                    message.Image.Url,
                    message.Image.Link);
                return;
            }

            logger.LogInformation(
                "Using image URL for download. MessageId: {MessageId}, FinalUrl: {FinalUrl}",
                message.Id,
                imageUrl);

            var mediaBytes = await whatsAppMediaService.DownloadMediaFromUrlAsync(
                imageUrl,
                cancellationToken);

            logger.LogInformation(
                "Image downloaded successfully. MessageId: {MessageId}, Size: {Size} bytes",
                message.Id,
                mediaBytes.Length);

            var base64Image = Convert.ToBase64String(mediaBytes);

            var latitude = (decimal)(pendingLatitude ?? DefaultLatitude);
            var longitude = (decimal)(pendingLongitude ?? DefaultLongitude);

            if (!pendingLatitude.HasValue)
            {
                logger.LogWarning(
                    "No location provided. Using default location. Latitude: {Latitude}, Longitude: {Longitude}",
                    latitude,
                    longitude);
            }

            logger.LogInformation(
                "Analyzing image with AI. MessageId: {MessageId}",
                message.Id);

            using var imageStream = new MemoryStream(mediaBytes);
            var analysisResult = await incidentAnalysisService.AnalyzeImageAsync(
                imageStream,
                cancellationToken);

            if (analysisResult.IsFailure)
            {
                logger.LogError(
                    "AI analysis failed. MessageId: {MessageId}, Error: {Error}",
                    message.Id,
                    analysisResult.Error.Name);
                return;
            }

            var analysis = analysisResult.Value;

            logger.LogInformation(
                "AI analysis completed. Category: {Category}, Subcategory: {Subcategory}, Description: {Description}",
                analysis.CategoryId,
                analysis.SubcategoryId,
                analysis.Description);

            var fileName = $"{Guid.NewGuid()}.jpg";
            var filePath = $"whatsapp/{message.From}/{fileName}";

            var file = Domain.Common.File.File.CreateForSave(
                fileName,
                base64Image,
                filePath,
                IncidentImagesBucket,
                message.Image.MimeType);

            var storageResult = await storageService.SaveFile(file, cancellationToken);

            if (storageResult.IsFailure)
            {
                logger.LogError(
                    "Failed to save image to storage. MessageId: {MessageId}, Error: {Error}",
                    message.Id,
                    storageResult.Error.Name);
                return;
            }

            logger.LogInformation(
                "Image saved to storage. FilePath: {FilePath}",
                filePath);

            var leader = await leaderRepository.GetNearestLeaderAsync(
                latitude,
                longitude,
                cancellationToken);

            if (leader is null)
            {
                logger.LogWarning(
                    "No leader found near location. Latitude: {Latitude}, Longitude: {Longitude}",
                    latitude,
                    longitude);
                return;
            }

            var radicateNumber = GenerateRadicateNumber();

            logger.LogInformation(
                "Generated radicateNumber. Value: {RadicateNumber}, Length: {Length}",
                radicateNumber,
                radicateNumber.Length);
            var location = Location.Create(latitude, longitude);

            var incident = Incident.Create(
                radicateNumber,
                filePath,
                location,
                leader.MunicipalityId,
                leader.Id,
                null,
                message.Image.Caption);

            incident.SetAnalysis(analysis);

            incidentRepository.Add(incident);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Incident created successfully. IncidentId: {IncidentId}, RadicateNumber: {RadicateNumber}, LeaderId: {LeaderId}, Phone: {Phone}",
                incident.Id,
                radicateNumber,
                leader.Id,
                message.From);

            await SendRadicateNumberToWhatsAppAsync(message.From, radicateNumber, cancellationToken);

            pendingLatitude = null;
            pendingLongitude = null;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error processing image and creating incident. MessageId: {MessageId}",
                message.Id);
        }
    }

    private static string GenerateRadicateNumber()
    {
        var uuid = Guid.NewGuid().ToString("N")[..4].ToUpper();
        return $"INC-RA-{uuid}";
    }

    private async Task SendRadicateNumberToWhatsAppAsync(string phoneNumber, string radicateNumber, CancellationToken cancellationToken)
    {
        try
        {
            var message = $"Incidente reportado con identificador: {radicateNumber}";
            
            var result = await whatsAppMessagingService.SendTextMessageAsync(
                phoneNumber,
                message,
                cancellationToken);

            if (result.IsFailure)
            {
                logger.LogWarning(
                    "Failed to send radicateNumber via WhatsApp. PhoneNumber: {PhoneNumber}, RadicateNumber: {RadicateNumber}, Error: {Error}",
                    phoneNumber,
                    radicateNumber,
                    result.Error.Name);
            }
            else
            {
                logger.LogInformation(
                    "RadicateNumber sent successfully via WhatsApp. PhoneNumber: {PhoneNumber}, RadicateNumber: {RadicateNumber}",
                    phoneNumber,
                    radicateNumber);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error sending radicateNumber via WhatsApp. PhoneNumber: {PhoneNumber}, RadicateNumber: {RadicateNumber}",
                phoneNumber,
                radicateNumber);
        }
    }
}
