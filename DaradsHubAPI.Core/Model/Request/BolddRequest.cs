namespace DaradsHubAPI.Core.Model.Request;
internal class BolddRequest
{
}
public record VirtualAccountPayload
{
    public string trackingid { get; set; }
    public string firstname { get; set; }
    public string lastname { get; set; }
    public string userbvn { get; set; }
    public string useremail { get; set; }
    public string userphone { get; set; }
    public string bankcode { get; set; }
}