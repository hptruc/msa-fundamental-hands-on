namespace MSA.Common.Contracts.Application.Common.Models;

public record BaseEvent
{
    public Guid CorrelationId { get; set; }
    public UserIdentityModel? UserIdentity { get; set; }
}