namespace Urban.AI.Infrastructure.Auth.Authentication.Models;

internal sealed class ClientRepresentationModel
{
    public string Id { get; set; }
    public string ClientId { get; set; }
    public string Name { get; set; }
    public bool Enabled { get; set; }
}
