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