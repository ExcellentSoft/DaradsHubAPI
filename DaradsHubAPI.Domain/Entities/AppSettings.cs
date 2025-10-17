namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public class AppSettings
{
    public string E_Key { get; set; }
    public string E_IV { get; set; }
    public string EncryptionKey { get; set; }
    public string SendGridEncryptedKey { get; set; }
    public string EmailFrom { get; set; }
    public string MailDisplayName { get; set; }
    public string ReplyTo { get; set; }
    public string JwtKey { get; set; }
    public string APIKEY { get; set; }
    public string WhatsAppKey { get; set; }
    public string WhatsAppUri { get; set; }
    public string ReferralSignUpUrl { get; set; }
    //..........LeadTech
    public string LeadBaseUrl { get; set; }
    public string LeadApiKey { get; set; }
    public string SmsPoolBaseUrl { get; set; }
    public string SmsPoolApiKey { get; set; }
    public string Issuer { get; set; }
    public int OtpExpiry { get; set; }
    public int CacheExpiry { get; set; }
    public double Lifetime { get; set; }
    public string X_RapidAPI_Key { get; set; }
    public string X_RapidAPI_Host { get; set; }
    public string X_BaseUrl { get; set; }
    public string Boldd_PublicKey { get; set; }
    public string Boldd_SecretKey { get; set; }
    public string Boldd_URL { get; set; }
    public string CryptoCloud_URL { get; set; }
    public string CryptoCloud_APIKey { get; set; }
    public string Shop_Id { get; set; }
    public string CoinMarket_APIKey { get; set; }
    public string VendorWebAppUrl { get; set; }
    public string VPayBaseUrl { get; set; }
    public string VPayPublicKey { get; set; }
    public string VPayUserName { get; set; }
    public string VPayPassword { get; set; }
    public string PaymentioAPIKey { get; set; }
    public string PaymentioApiSecret { get; set; }
    public string PaymentioBusinessId { get; set; }
}