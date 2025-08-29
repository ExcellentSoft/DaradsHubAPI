using System.ComponentModel.DataAnnotations;

namespace DaradsHubAPI.Domain.Entities;

#nullable disable
public class Country
{
    public int id { get; set; }//add, countryId
    public string Name { get; set; }//=name
    public string Short_Name { get; set; } //=code
    public string Region { get; set; } //add, serviveType=N,SM
}
public class CountryList
{
    [Key]
    public int id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public int CountryId { get; set; }
    public string Telcode { get; set; }
    public string ServiceType { get; set; }
}
