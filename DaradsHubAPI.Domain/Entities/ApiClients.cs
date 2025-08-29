using System;
using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;

#nullable disable
public class ApiClients
{
    [Key]
    public int Id { get; set; }

    public bool IsConfirmed { get; set; }

    public string Email { get; set; }
    public string ApiKey { get; set; }

    public string SecretKey { get; set; }

    public string UserId { get; set; }
    public DateTime CreatedDate { get; set; }
}

//client customers
public class ClientCustomers
{
    [Key]
    public int Id { get; set; }
    public int ApiClientId { get; set; }
    [Required]
    public string ApiKey { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime CreatedDate { get; set; }
}
