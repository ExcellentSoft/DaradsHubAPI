using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Core.Model.Response;
#nullable disable
internal class BolddResponse
{
}

public sealed record BankDetails
{
    public string bankname { get; set; }
    public string bankcode { get; set; }
}
public record BolddApiResponse
{
    public bool status { get; set; }
    public string message { get; set; }
}

public sealed record BankResponse : BolddApiResponse
{
    public List<BankDetails> data { get; set; }
}
public sealed record VirtualAccountResponse : BolddApiResponse
{
    public string trackingref { get; set; }
    public string trackingid { get; set; }
    public string acctname { get; set; }
    public string acctno { get; set; }
    public string clientid { get; set; }
    public string bankcode { get; set; }
    public string bankname { get; set; }
}
