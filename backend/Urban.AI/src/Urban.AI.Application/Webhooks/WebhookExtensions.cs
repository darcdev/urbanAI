using Urban.AI.Application.Webhooks.Dtos;

namespace Urban.AI.Application.Webhooks;

internal static class WebhookExtensions
{
    public static ProcessWebhookResponse ToSuccessResponse(this List<string> processedMessageIds)
    {
        return new ProcessWebhookResponse
        {
            Success = true,
            ProcessedMessages = processedMessageIds.Count,
            ProcessedMessageIds = processedMessageIds
        };
    }

    public static ProcessWebhookResponse ToErrorResponse(this string errorMessage)
    {
        return new ProcessWebhookResponse
        {
            Success = false,
            ProcessedMessages = 0,
            ErrorMessage = errorMessage
        };
    }
}
