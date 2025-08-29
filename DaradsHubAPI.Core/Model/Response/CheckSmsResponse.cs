namespace DaradsHubAPI.Core.Model.Response;

#nullable disable
public class CheckSmsDto
{
    public int status { get; set; } //=1 pending, 6, refunded
    public string sms { get; set; }
    public string full_sms { get; set; }
    public long expiration { get; set; }
    public string message { get; set; }
    public int resend { get; set; }
    public int time_left { get; set; }
}
public class CheckSmsparam
{
    public string OrderId { get; set; }
    public string Key { get; set; }

    public int status { get; set; }
}

public class PurchaseSmsDto
{
    public int success { get; set; }
    public long number { get; set; }
    public string cc { get; set; }
    public string phonenumber { get; set; }
    public string order_id { get; set; }
    public string country { get; set; }
    public string service { get; set; }
    public int pool { get; set; }
    public int expires_in { get; set; }
    public int expiration { get; set; }
    public string message { get; set; }
    public string cost { get; set; }
    public int cost_in_cents { get; set; }
}

public class OrderHistoryDto
{
    public SmsOrderDataDto[] Data { get; set; }
}

public class SmsOrderDataDto
{
    public string cost { get; set; }
    public string order_code { get; set; }
    public string phonenumber { get; set; }
    public string code { get; set; }
    public string full_code { get; set; }
    public string short_name { get; set; }
    public string service { get; set; }
    public string status { get; set; }
    public int pool { get; set; }
    public string timestamp { get; set; }
    public string completed_on { get; set; }
    public long expiry { get; set; }
    public int time_left { get; set; }
}
public class SmsOrderHistoryParam
{
    public string Start { get; set; } = "0";
    public int Length { get; set; } = 1000;
    public string Search { get; set; }
}