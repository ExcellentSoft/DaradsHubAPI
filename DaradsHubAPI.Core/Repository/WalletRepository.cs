using CloudinaryDotNet.Actions;
using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
using DaradsHubAPI.Shared.Customs;
using DaradsHubAPI.Shared.Static;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using static DaradsHubAPI.Domain.Enums.Enum;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DaradsHubAPI.Core.Repository;
public class WalletRepository(AppDbContext _context) : GenericRepository<wallettb>(_context), IWalletRepository
{

    public async Task<AgentBalanceResponse> GetAgentWalletBalance(string email)
    {
        var balance = await _context.wallettb.Where(e => e.UserId == email).Select(c => new AgentBalanceResponse
        {
            Balance = c.Balance,
            LastUpdate = c.UpdateDate,

        }).FirstOrDefaultAsync();

        return balance ?? new AgentBalanceResponse { };
    }

    public async Task CreateHubWithdrawalRequest(HubWithdrawalRequest request)
    {
        _context.HubWithdrawalRequests.Add(request);
        await _context.SaveChangesAsync();
    }

    public async Task<List<WithdrawalRequestResponse>> GetWithdrawalRequests(int agentId)
    {
        var requestResponses = await (from req in _context.HubWithdrawalRequests
                                      where req.AgentId == agentId
                                      orderby req.DateCreated descending
                                      select new WithdrawalRequestResponse
                                      {
                                          Id = req.Id,
                                          Amount = req.Amount,
                                          DateCreated = req.DateCreated,
                                          RefNumber = req.ReferenceNumber,
                                          Status = req.Status.GetDescription(),
                                      }).Take(10).ToListAsync();

        return requestResponses;
    }

    public async Task<List<WithdrawalRequestResponse>> GetAllWithdrawalRequests(WithdrawalRequest request)
    {
        var query = from req in _context.HubWithdrawalRequests
                    join u in _context.userstb on req.AgentId equals u.id
                    where (request.StartDate == null || req.DateCreated >= request.StartDate.Value.Date)
                    && (request.EndDate == null || req.DateCreated <= request.EndDate.Value.Date)
                    orderby req.DateCreated descending
                    select new WithdrawalRequestResponse
                    {
                        Id = req.Id,
                        Amount = req.Amount,
                        DateCreated = req.DateCreated,
                        RefNumber = req.ReferenceNumber,
                        AgentName = u.fullname,
                        Status = req.Status.GetDescription(),
                        StatusEnum = req.Status,
                    };
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            request.SearchText = request.SearchText.Trim().ToLower();
            query = query.Where(d => d.AgentName != null && d.AgentName.ToLower().Contains(request.SearchText));
        }

        if (request.Status is not null)
        {
            query = query.Where(d => d.StatusEnum == request.Status);
        }

        var requestResponses = await query.Skip((request!.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return requestResponses;
    }

    public async Task<SingleWithdrawalRequestResponse> GetWithdrawalRequest(long withdrawalRequestId)
    {
        var requestResponses = await (from req in _context.HubWithdrawalRequests
                                      where req.Id == withdrawalRequestId
                                      join u in _context.userstb on req.AgentId equals u.id
                                      orderby req.DateCreated descending
                                      select new SingleWithdrawalRequestResponse
                                      {
                                          Id = req.Id,
                                          Amount = req.Amount,
                                          DateCreated = req.DateCreated,
                                          RefNumber = req.ReferenceNumber,
                                          AccountNumber = req.AccountNumber,
                                          AccountName = req.AccountName,
                                          BankName = req.BankName,
                                          AgentName = u.fullname,
                                          Status = req.Status.GetDescription(),
                                      }).FirstOrDefaultAsync();

        return requestResponses ?? new SingleWithdrawalRequestResponse { };
    }

    public async Task<(bool status, string message)> ChangeWithdrawalRequests(ChangeWithdrawalRequestStatus requestStatus)
    {
        var today = GetLocalDateTime.CurrentDateTime();
        var request = await _context.HubWithdrawalRequests.Where(s => s.Id == requestStatus.WithdrawalRequestId).FirstOrDefaultAsync();
        if (request is null)
        {
            return new(false, "Withdrawal request not found.");
        }

        if (request.Status == WithdrawalRequestStatus.Paid)
        {
            return new(false, $"Payment has been made on {request.DateUpdated:yyyy MMMM, dd}");
        }

        if (requestStatus.Status == WithdrawalRequestStatus.Paid)
        {
            var agent = await _context.userstb.FirstOrDefaultAsync(d => d.id == request.AgentId);
            var customerWallet = await _context.wallettb.FirstOrDefaultAsync(r => r.UserId == agent!.email);
            if (customerWallet is null)
                return new(false, "Agent wallet record not found.");

            customerWallet.Balance -= request.Amount;
            customerWallet.UpdateDate = GetLocalDateTime.CurrentDateTime();

            await _context.SaveChangesAsync();

            var refNo = string.Concat("wallet-debit", "-", CustomizeCodes.ReferenceCode().AsSpan(0, 5));
            var walletTransaction = new GwalletTran
            {
                DR = request.Amount,
                walletBal = customerWallet.Balance.GetValueOrDefault(),
                amt = request.Amount,
                userName = agent!.email,
                refNo = refNo,
                transMedium = "Wallet",
                transdate = GetLocalDateTime.CurrentDateTime(),
                transStatus = "D",
                transType = "DebitWallet",
                Status = "Complete",
                areaCode = request.AgentId.ToString()
            };

            _context.GwalletTrans.Add(walletTransaction);

            request.Status = WithdrawalRequestStatus.Paid;
            request.DateUpdated = today;
            await _context.SaveChangesAsync();
        }

        request.Status = requestStatus.Status;
        request.DateUpdated = today;
        await _context.SaveChangesAsync();
        return new(true, "Successful.");
    }
}
