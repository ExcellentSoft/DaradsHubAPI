using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Core.IRepository;
public interface IWalletRepository : IGenericRepository<wallettb>
{
    Task CreateHubWithdrawalRequest(HubWithdrawalRequest request);
    Task<AgentBalanceResponse> GetAgentWalletBalance(string email);
    Task<List<WithdrawalRequestResponse>> GetWithdrawalRequests(int agentId);
}
