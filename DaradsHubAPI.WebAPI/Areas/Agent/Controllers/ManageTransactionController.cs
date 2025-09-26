using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Services.Concrete;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Areas.Agent.Controllers;


[Tags("Agent")]
public class ManageTransactionController(IWalletTransactionService _walletTransaction) : ApiBaseController
{
    [HttpGet("agent-wallet-transactions")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<WalletTransactionRecords>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentWalletTransactions([FromQuery] TransactionListRequest request)
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _walletTransaction.GetAgentWalletTransactions(request, email);
        return ResponseCode(response);
    }

    [HttpGet("agent-wallet-balance")]
    [ProducesResponseType(typeof(ApiResponse<AgentBalanceResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAgentBalance()
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _walletTransaction.GetAgentBalance(email);
        return ResponseCode(response);
    }

    [HttpPost("create-withdrawal-request")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreateWithdrawalRequest([FromBody] CreateWithdrawalRequest request)
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var agentId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _walletTransaction.CreateWithdrawalRequest(request, email, agentId);
        return ResponseCode(response);
    }

    [HttpGet("withdrawal-requests")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<WithdrawalRequestResponse>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetWithdrawalRequests()
    {
        var agentId = int.Parse(User.Identity?.GetUserId() ?? "");
        var response = await _walletTransaction.GetWithdrawalRequests(agentId);
        return ResponseCode(response);
    }
}