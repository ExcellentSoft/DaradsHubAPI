using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Model;

public class ApiResponse<TResponse>
{
    /// <summary>
    /// The status of request,if true request run completed and expected logic validations and response else there is info, warning or error message 
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Status { get; set; } = false;
    /// <summary>
    /// The statusCode to determine response message to display at client side
    /// </summary>
    public StatusEnum StatusCode { get; set; }
    /// <summary>
    /// Additional information on the reponse.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Message { get; set; }
    /// <summary>
    /// The total record for the list
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int TotalRecord { get; set; }
    /// <summary>
    /// Number of pages inline with pagesize 
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Pages { get; set; }
    /// <summary>
    /// Record current page number on view
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int CurrentPageCount { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int CurrentPage { get; set; }
    /// <summary>
    /// The contained data.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public TResponse Data { get; set; }

    /// <summary>
    /// A list of error messages for errors that occured during processing.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<string>? Errors { get; set; }

    public bool IsEmpty() => (Errors == null || Errors.Count == 0) && Data == null && Message == null;
}

public class ApiResponse
{
    public ApiResponse(string _message, StatusEnum _statusCode, bool? _status)
    {
        Message = _message;
        StatusCode = _statusCode;
        Status = _status;
    }
    public bool? Status { get; set; } = false;
    public string Message { get; private set; }
    public StatusEnum StatusCode { get; private set; }
}

public class IdNameRecord
{
    public string? Name { get; set; }
    public long Id { get; set; }
}
public class NameValueRecord
{
    public string? Name { get; set; }
    public string? Value { get; set; }
}

public record ListRequest
{
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
    public string? SearchText { get; set; }
}

public record ProductListRequest
{
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
    public string? SearchText { get; set; }
}

public class AgentProductListRequest
{
    public string? SearchText { get; set; }
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
    public int AgentId { get; set; }
    public int CategoryId { get; set; }
}

public class AgentDigitalProductListRequest
{
    public string? SearchText { get; set; }
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
    public int AgentId { get; set; }
    public int CatalogueId { get; set; }
}