using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Shared.Customs;
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

    public async Task<ApiResponse<IEnumerable<AgentWalletTransactionRecord>>> GetAgentWalletTransactions(TransactionListRequest request, string email)
    {
        var query = _unitOfWork.WalletTransactions.GetAgentWalletTransactions(email);

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            query = query
                .Where(s => s.Type != null && s.Type.ToLower().Contains(request.SearchText.ToLower()) || s.ReferenceNumber != null && s.ReferenceNumber.ToLower().Contains(request.SearchText.ToLower()));
        }

        var totalTransactions = query.Count();
        var paginatedTransactions = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize).ToListAsync();

        return new ApiResponse<IEnumerable<AgentWalletTransactionRecord>> { Message = "Wallet transactions fetched successfully.", Status = true, Data = paginatedTransactions, StatusCode = StatusEnum.Success, TotalRecord = totalTransactions, Pages = request.PageSize, CurrentPageCount = request.PageNumber };
    }

    public async Task<ApiResponse<AgentBalanceResponse>> GetAgentBalance(string email)
    {
        var balanceResponse = await _unitOfWork.Wallets.GetAgentWalletBalance(email);

        return new ApiResponse<AgentBalanceResponse> { Message = "Agent balance fetched successfully.", Status = true, Data = balanceResponse, StatusCode = StatusEnum.Success };
    }

    public async Task<ApiResponse> CreateWithdrawalRequest(CreateWithdrawalRequest model, string email, int agentId)
    {
        var wallet = await _unitOfWork.Wallets.GetSingleWhereAsync(e => e.UserId == email);
        if (wallet is null)
        {
            return new ApiResponse("Wallet record not found.", StatusEnum.Validation, false);
        }

        if (model.Amount > wallet.Balance)
        {
            return new ApiResponse("The withdrawal amount cannot exceed the current balance.", StatusEnum.Validation, false);
        }

        var refNumber = $"REF{CustomizeCodes.GenerateOTP(4)}";
        var request = new HubWithdrawalRequest
        {
            AgentId = agentId,
            Amount = model.Amount,
            DateCreated = GetLocalDateTime.CurrentDateTime(),
            DateUpdated = GetLocalDateTime.CurrentDateTime(),
            AccountName = model.AccountName,
            AccountNumber = model.AccountNumber,
            BankName = model.Bank,
            Status = WithdrawalRequestStatus.Processing,
            ReferenceNumber = refNumber,

        };

        await _unitOfWork.Wallets.CreateHubWithdrawalRequest(request);
        return new ApiResponse("Withdrawal request created successfully.", StatusEnum.Success, true);
    }

    public async Task<ApiResponse<IEnumerable<WithdrawalRequestResponse>>> GetWithdrawalRequests(int agentId)
    {
        var requests = await _unitOfWork.Wallets.GetWithdrawalRequests(agentId);

        return new ApiResponse<IEnumerable<WithdrawalRequestResponse>> { Message = "Wallet requests fetched successfully.", Status = true, Data = requests, StatusCode = StatusEnum.Success };
    }
}