namespace LTC.AdministrationService.Options;

public class InternalApiOptions
{
    public const string SectionName = "InternalApi";

    /// <summary>Shared secret for customer-service (and other internal callers).</summary>
    public string CustomerServiceApiKey { get; set; } = string.Empty;
}
