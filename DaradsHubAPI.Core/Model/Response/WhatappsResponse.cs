using Newtonsoft.Json;

namespace DaradsHubAPI.Core.Models;
#nullable disable
internal class WhatappsResponse
{
}
public partial class WhatsappResponseDto
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("data")]
    public WhatsAppData Data { get; set; }

    [JsonProperty("ResponseMessage")]
    public string ResponseMessage { get; set; }

    [JsonProperty("ErrorCode")]
    public string ErrorCode { get; set; }

    [JsonProperty("ErrorMessage")]
    public string ErrorMessage { get; set; }


}
public class WhatsappParam
{
    [JsonProperty("to_number")]
    public string ToNumber { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("Username")]
    public string Username { get; set; }

    [JsonProperty("Password")]
    public string Password { get; set; }
}
public partial class WhatsappResponseDto
{
    public static WhatsappResponseDto FromJson(string json) => JsonConvert.DeserializeObject<WhatsappResponseDto>(json);
}
public partial class WhatsAppData
{
    [JsonProperty("chatId")]
    public string ChatId { get; set; }

    [JsonProperty("msgId")]
    public string MsgId { get; set; }
}
public class PayIOWebhookPayload
{
    [JsonProperty("notification_status")]
    public string NotificationStatus { get; set; }

    [JsonProperty("transaction_id")]
    public string TransactionId { get; set; }

    [JsonProperty("amount_paid")]
    public decimal AmountPaid { get; set; }

    [JsonProperty("settlement_amount")]
    public decimal SettlementAmount { get; set; }

    [JsonProperty("settlement_fee")]
    public decimal SettlementFee { get; set; }

    [JsonProperty("transaction_status")]
    public string TransactionStatus { get; set; }

    [JsonProperty("sender")]
    public PayIOWebhookParty Sender { get; set; }

    [JsonProperty("receiver")]
    public PayIOWebhookParty Receiver { get; set; }

    [JsonProperty("customer")]
    public PayIOWebhookCustomer Customer { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; }
}

public class PayIOWebhookParty
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("account_number")]
    public string AccountNumber { get; set; }

    [JsonProperty("bank")]
    public string Bank { get; set; }
}

public class PayIOWebhookCustomer
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("phone")]
    public string Phone { get; set; }

    [JsonProperty("customer_id")]
    public string CustomerId { get; set; }
}

public class PaymentIOApiResponse
{
    public string status { get; set; }
    public string message { get; set; }
    public Customerp customer { get; set; }
    public Business business { get; set; }
    public List<Bankaccount2> bankAccounts { get; set; }
    public object[] errors { get; set; }
}

public class Customerp
{
    public string customer_id { get; set; }
    public string customer_name { get; set; }
    public string customer_email { get; set; }
    public string customer_phone_number { get; set; }
}

public class Business
{
    public string business_name { get; set; }
    public string business_email { get; set; }
    public string business_phone_number { get; set; }
    public object business_Id { get; set; }
}

public class Bankaccount2
{
    public string bankCode { get; set; }
    public string accountNumber { get; set; }
    public string accountName { get; set; }
    public string bankName { get; set; }
    public string Reserved_Account_Id { get; set; }
    public string Message { get; set; }
}

public class CreateVirtualAccParam
{ //Use User Id to get all values where it is been called 
    public string email { get; set; }
    public string name { get; set; }
    public string phoneNumber { get; set; }
    public List<string> bankCode { get; set; }
    public string businessId { get; set; }
    //public string? accessId { get; set; } = "Web";//e.g Web, TeleBot, WhatsAppBot
}