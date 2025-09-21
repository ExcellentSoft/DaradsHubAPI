using DaradsHubAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DaradsHubAPI.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public virtual DbSet<admin_notification> admin_notification { get; set; }
    public virtual DbSet<adverttb> adverttbs { get; set; }
    public virtual DbSet<agenttb> agenttbs { get; set; }
    public virtual DbSet<customertb> customertbs { get; set; }
    public virtual DbSet<feedbacktb> feedbacktbs { get; set; }
    public virtual DbSet<category> categories { get; set; }
    public virtual DbSet<GwalletTran> GwalletTrans { get; set; }
    public virtual DbSet<locationtb> locationtbs { get; set; }
    public virtual DbSet<Paymenttb> Paymenttb { get; set; }
    public virtual DbSet<requesttb> requesttb { get; set; }
    public virtual DbSet<searchlog> searchlog { get; set; }
    public virtual DbSet<servy> servy { get; set; } //Ads
    public virtual DbSet<state> state { get; set; }
    public virtual DbSet<states_new> states_new { get; set; }
    public virtual DbSet<SubscriptionCost> SubscriptionCost { get; set; }
    public virtual DbSet<subscriptionplan> subscriptionplan { get; set; }
    public virtual DbSet<GiftCardOrders> GiftCardOrders { get; set; }
    public virtual DbSet<userstb> userstb { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<wallettb> wallettb { get; set; }
    public virtual DbSet<Withdrawtb> Withdrawtb { get; set; }
    public virtual DbSet<Logapp_error> Logapp_error { get; set; }
    public virtual DbSet<PaymentLog> PaymentLog { get; set; }
    public virtual DbSet<webhookLog> webhookLog { get; set; }
    public virtual DbSet<Influencertb> Influencertb { get; set; }
    public virtual DbSet<ApiClients> ApiClients { get; set; }
    public virtual DbSet<AuditTrail> AuditTrail { get; set; }
    public virtual DbSet<shopCat> shopCats { get; set; }
    public virtual DbSet<ClientCustomers> ClientCustomers { get; set; }
    public virtual DbSet<AllLogs> AllLogs { get; set; }
    public virtual DbSet<LogTypes> LogTypes { get; set; }
    public virtual DbSet<MessageAudiences> MessageAudiences { get; set; }
    public virtual DbSet<MessageEmailTemplates> MessageEmailTemplates { get; set; }
    public virtual DbSet<MessagesSentLogs> MessagesSentLogs { get; set; }
    public virtual DbSet<MessagesSent> MessagesSent { get; set; }
    public virtual DbSet<MessageThread> MessageThreads { get; set; }
    public virtual DbSet<referralCommission> ReferralCommission { get; set; }
    public virtual DbSet<UserreferralBalance> UserreferralBalance { get; set; }
    public virtual DbSet<WithdrawCommissionHistory> WithdrawCommissionHistory { get; set; }
    public virtual DbSet<VendorWallet> VendorWallet { get; set; }
    public virtual DbSet<VendorWalletTrans> VendorWalletTrans { get; set; }
    public virtual DbSet<Vendorpaymenthistory> Vendorpaymenthistory { get; set; }
    public virtual DbSet<SetvendorLog> SetvendorLog { get; set; }
    public virtual DbSet<NumberOrders> NumberOrders { get; set; }
    public virtual DbSet<numberOrderRate> numberOrderRate { get; set; }
    public virtual DbSet<CountryList> CountryList { get; set; }
    public virtual DbSet<RequestLog> RequestLogs { get; set; }
    public virtual DbSet<VendorWebsite> VendorWebsites { get; set; }
    public virtual DbSet<VendorUploadRequestLog> VendorUploadRequestLogs { get; set; }
    public virtual DbSet<VendorPaymentGateway> VendorPaymentGateways { get; set; }
    public virtual DbSet<UserBankDetails> UserBankDetails { get; set; }
    public virtual DbSet<VendorCustomerOrder> VendorCustomerOrders { get; set; }
    public virtual DbSet<VendorCustomerTransaction> VendorCustomerTransactions { get; set; }
    public virtual DbSet<SiteTransaction> SiteTransactions { get; set; }
    public virtual DbSet<VendorCustomer> VendorCustomers { get; set; }
    public virtual DbSet<VendorCustomerPaymentProof> VendorCustomerPaymentProofs { get; set; }
    public virtual DbSet<VendorCustomerFeedback> VendorCustomerFeedbacks { get; set; }
    public virtual DbSet<VendorCustomerWallet> VendorCustomerWallets { get; set; }
    public virtual DbSet<SiteWallet> SiteWallets { get; set; }
    public virtual DbSet<OtpVerificationLog> OtpVerificationLogs { get; set; }
    public virtual DbSet<RentNumbers> RentNumbers { get; set; }
    public virtual DbSet<CustomerTempEmail> CustomerTempEmails { get; set; }
    public virtual DbSet<CustomerMessage> CustomerMessages { get; set; }
    public virtual DbSet<CryptoCloudUser> CryptoCloudUsers { get; set; }
    public virtual DbSet<CustomerVirtualAccount> CustomerVirtualAccounts { get; set; }
    public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
    public virtual DbSet<VendorSubscriptionPlan> VendorSubscriptionPlans { get; set; }
    public virtual DbSet<MKTLogType> MKTLogTypes { get; set; }
    public virtual DbSet<MKTLog> MKTLogs { get; set; }
    public virtual DbSet<MKTLogOrder> MKTLogOrders { get; set; }
    public virtual DbSet<ShippingAddress> ShippingAddresses { get; set; }
    public virtual DbSet<Review> Reviews { get; set; }
    public virtual DbSet<VpayApiLogin> VpayApiLogins { get; set; }
    public virtual DbSet<SubCategory> SubCategories { get; set; }
    public virtual DbSet<HubProduct> HubProducts { get; set; }
    public virtual DbSet<HubAgentProduct> HubAgentProducts { get; set; }
    public virtual DbSet<ProductImages> ProductImages { get; set; }
    public virtual DbSet<HubAgentProfile> HubAgentProfiles { get; set; }
    public virtual DbSet<Catalogue> Catalogues { get; set; }
    public virtual DbSet<CatalogueMapping> CatalogueMappings { get; set; }
    public virtual DbSet<HubDigitalProduct> HubDigitalProducts { get; set; }
    public virtual DbSet<DigitalProductImages> DigitalProductImages { get; set; }
    public virtual DbSet<ProductRequestImages> ProductRequestImages { get; set; }
    public virtual DbSet<HubReview> HubReviews { get; set; }
    public virtual DbSet<HubAgentReview> HubAgentReviews { get; set; }
    public virtual DbSet<HubOrder> HubOrders { get; set; }
    public virtual DbSet<HubOrderItem> HubOrderItems { get; set; }
    public virtual DbSet<HubOrderTracking> HubOrderTracking { get; set; }
    public virtual DbSet<HubProductRequest> HubProductRequests { get; set; }
    public virtual DbSet<HubNotification> HubNotifications { get; set; }
    public virtual DbSet<HubFAQ> HubFAQs { get; set; }
    public virtual DbSet<HubChatConversation> HubChatConversations { get; set; }
    public virtual DbSet<HubChatMessage> HubChatMessages { get; set; }

}
