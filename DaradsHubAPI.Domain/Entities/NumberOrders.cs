using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public class NumberOrders
{
    [Key]
    public int Id { get; set; }
    public string Userid { get; set; }
    public int TargetId { get; set; }
    public string refNumber { get; set; }
    public decimal Actualamt { get; set; }
    public decimal ChargeAmount { get; set; }
    public string Number { get; set; }
    public DateTime CreatedDate { get; set; }
    public string TargetName { get; set; }
    public string VerificationId { get; set; }
    public string VerificationCode { get; set; }
    public string Status { get; set; }
    public string NumType { get; set; }
    public string ServiceCode { get; set; }


}
public class RentNumbers
{
    [Key]
    public int Id { get; set; }
    public string Userid { get; set; }
    public int ServiceId { get; set; }
    public string refNumber { get; set; }
    public string RentalCode { get; set; }

    public decimal Actualamt { get; set; }
    public decimal ChargeAmount { get; set; }
    public string Number { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Status { get; set; }
    public string RentMessage { get; set; }
    public int Days { get; set; }
    public int ActiveFor { get; set; }
    public string Expiry { get; set; }
}
