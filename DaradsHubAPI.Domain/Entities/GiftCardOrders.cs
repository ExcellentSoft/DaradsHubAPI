using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Domain.Entities;

#nullable disable
public partial class GiftCardOrders
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    public int TransId { get; set; }
    public string RefNo { get; set; }
    public double TotalPriceInNaira { get; set; }
    public decimal TransAmount { get; set; }
    public int TotalPrice { get; set; }
    public int UnitPrice { get; set; }
    public int ProductId { get; set; }
    public int Qty { get; set; }
    public string CardNumber { get; set; }
    public string PINCode { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public string ProductName { get; set; }
}
