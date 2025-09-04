using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Shared.Interface;
using Microsoft.EntityFrameworkCore;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Services.Concrete;
public class WalletTransactionService(IUnitOfWork _unitOfWork, IFileService _fileService) : IWalletTransactionService
{

    public async Task<ApiResponse<IEnumerable<WalletTransactionRecords>>> GetWalletTransactions(TransactionListRequest request, string email)
    {
        var query = _unitOfWork.WalletTransactions.GetWalletTransactions(email);

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            query = query
                .Where(s => s.Type != null && s.Type.ToLower().Contains(request.SearchText.ToLower()) || s.ReferenceNumber != null && s.ReferenceNumber.ToLower().Contains(request.SearchText.ToLower()));
        }
        var totalTransactions = query.Count();
        var paginatedTransactions = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize).ToListAsync();


        return new ApiResponse<IEnumerable<WalletTransactionRecords>> { Message = "Wallet transactions fetched successfully.", Status = true, Data = paginatedTransactions, StatusCode = StatusEnum.Success, TotalRecord = totalTransactions, Pages = request.PageSize, CurrentPageCount = request.PageNumber };
    }
}
