namespace Tawsella.Infrastructure.Models
{
    public class FawaterkSettings
    {
        public const string SectionName = "Fawaterk";
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string VendorKey { get; set; } = string.Empty;
        public string ProviderKey { get; set; } = string.Empty;
        public string SuccessRedirectUrl { get; set; } = string.Empty;
        public string FailRedirectUrl { get; set; } = string.Empty;
    }
}
