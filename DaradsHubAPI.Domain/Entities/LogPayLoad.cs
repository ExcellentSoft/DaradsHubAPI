using System;
using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public class FlutterPayLoadLog
{  //Table to save WebHook PayLoad
    [Key]
    public int Id { get; set; }
    public string WebHookEvent { get; set; }


    public string EventType { get; set; }
    //Add other Data

    public string Status { get; set; } //successful
    public double Amount { get; set; } //Paid Amount
    public double Charged_amount { get; set; }
    public long TransId { get; set; }
    public string tx_ref { get; set; }//tx_ref : Reference Number
    public string flw_ref { get; set; }
    public string device_fingerprint { get; set; }
    public string currency { get; set; }
    public string ip { get; set; }
    public string narration { get; set; }
    public string payment_type { get; set; }
    public double app_fee { get; set; }
    public double merchant_fee { get; set; }
    public string processor_response { get; set; }
    public string auth_model { get; set; }

    public DateTime created_at { get; set; }
    public long account_id { get; set; }
    //public CustomerData customer { get; set; }
    public string Name { get; set; }
    public string phone_number { get; set; }
    public string customer_id { get; set; }
    public string customer_code { get; set; }
    public string email { get; set; }

    // public CardData card { get; set; }
    public string first_6digits { get; set; }//tx_ref : Reference Number
    public string last_4digits { get; set; }
    public string issuer { get; set; }
    public string country { get; set; }
    public string type { get; set; }
    public string expiry { get; set; }

    //Transfer Data
    public string account_number { get; set; } //Account Number
    public string bank_name { get; set; }
    public string bank_code { get; set; }
    public string fullname { get; set; } //valid name  used on ID and BVN
    public string debit_currency { get; set; }
    public double fee { get; set; }
    public string reference { get; set; }
    public string meta { get; set; }
    public string approver { get; set; }
    public string complete_message { get; set; }
    public int requires_approval { get; set; }
    public int is_approved { get; set; }
    public DateTime? PayLoadDate { get; set; }

    //PayLoad : Event, EventType, Data
}
public class webhookLog
{
    [Key]
    public int Id { get; set; }
    public string WebHookEvent { get; set; }
    public string EventType { get; set; }
    //Add other Data
    [Required]
    public string tx_ref { get; set; }
    public string TransId { get; set; }//data.id
    [Required]
    public string Status { get; set; }
    [Required]
    public decimal Amount { get; set; }
    public string WalletUpdate { get; set; }//Y/N
    public string WalletNotUpdateReason { get; set; }
    public string narration { get; set; }
    public string payment_type { get; set; }
    [Required]
    public string CustomerEmail { get; set; }
    public string CustomerCode { get; set; }
    public DateTime LogDate { get; set; }//=Create_at
    public DateTime Create_at { get; set; }
    public string Pgateway { get; set; }
    public string ErrorMsg { get; set; }

}
