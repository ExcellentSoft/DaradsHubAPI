using Newtonsoft.Json;

namespace DaradsHubAPI.Core.Model.Request
{
    public class LoginDto
    {
        public bool Status { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? BusinessName { get; set; }

    }

    public class CreateCustomerParam
    {
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? contactfirstname { get; set; }
        public string? contactlastname { get; set; }
    }

    public record GetVPayCustomerAccount
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string Nuban { get; set; }
        public DateTime CreatedOn { get; set; }

    }
    public class CreateVPayCustomerRequest
    {
        public string PhoneNumber { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
    }

    public class CreateCustomerDto
    {
        public bool Status { get; set; }
        public string? Id { get; set; }
        public string? Message { get; set; }

    }

    public class LoginParam
    {
        public string password { get; set; } = default!;
        public string username { get; set; } = default!;
    }
}
