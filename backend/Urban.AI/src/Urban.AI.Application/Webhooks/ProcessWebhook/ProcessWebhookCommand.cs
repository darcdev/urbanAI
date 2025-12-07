using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Webhooks.Dtos;

namespace Urban.AI.Application.Webhooks.ProcessWebhook;

public record ProcessWebhookCommand(ProcessWebhookRequest Request) : ICommand<ProcessWebhookResponse>;
