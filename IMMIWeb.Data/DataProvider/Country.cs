using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("Country")]
public partial class Country
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(5)]
    public string? MobileCode { get; set; }

    public bool? IsActive { get; set; }

    [StringLength(5)]
    public string? Prefix { get; set; }

    [StringLength(5)]
    public string? Currency { get; set; }

    public bool? IsImmigrationCountry { get; set; }

    public double? ExchangeRate { get; set; }

    [InverseProperty("CountryNavigation")]
    public virtual ICollection<ConsultantServiceForCountry> ConsultantServiceForCountries { get; set; } = new List<ConsultantServiceForCountry>();

    [InverseProperty("CountryNavigation")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
