using DaradsHubAPI.Core.Model;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaradsHubAPI.WebAPI.Areas.Customer.Controllers;

[Tags("Customer")]
public class TransactionController(IWalletTransactionService _walletTransactionService) : ApiBaseController
{
    [HttpGet("wallet-transactions")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<WalletTransactionRecords>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetWalletTransactions([FromQuery] TransactionListRequest request)
    {
        var email = User.Identity?.GetUserEmail() ?? "";
        var response = await _walletTransactionService.GetWalletTransactions(request, email);
        return ResponseCode(response);
    }
}