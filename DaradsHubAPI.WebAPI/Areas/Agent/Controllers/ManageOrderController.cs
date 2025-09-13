using DaradsHubAPI.Core.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DaradsHubAPI.WebAPI.Areas.Agent.Controllers;


[Tags("Agent")]
public class ManageOrderController(IOrderService _orderService) : ApiBaseController
{
}
