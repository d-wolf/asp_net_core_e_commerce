namespace ECommerce.Utility;

public class StripeSettings
{
    public required string SecretKey { get; set; }
    public required string publishableKey { get; set; }
}