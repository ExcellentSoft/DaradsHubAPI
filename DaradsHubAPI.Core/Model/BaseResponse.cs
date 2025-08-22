using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Model;

public record BaseResponse(bool Status, string Message);
public record BaseResponse<T>(bool Status, string Message, T? Data);

public sealed record PaginatedData<T> where T : class
{
    public PaginatedData(IEnumerable<T> records, long totalRecordsCount, int page = 1, int pageSize = 10)
    {
        Records = records;
        CurrentPage = page;
        CurrentRecordCount = Records.Count();
        TotalRecordCount = totalRecordsCount;
        TotalPages = (int)Math.Round((decimal)(totalRecordsCount / pageSize), 0, MidpointRounding.ToPositiveInfinity) + 1;
    }

    public IEnumerable<T> Records { get; set; }
    public int CurrentPage { get; set; }
    public int CurrentRecordCount { get; set; }
    public long TotalRecordCount { get; set; }
    public int TotalPages { get; set; }
}
public class GenericListRequest
{
    public string? SearchText { get; set; }
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
}
public class CacheKeyVal
{
    public string key { get; set; } = default!;
    public string? value { get; set; }

}
public class OrderListRequest
{
    public string? SearchText { get; set; }
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
    public OrderStatus Status { get; set; }
}