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
}