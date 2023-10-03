using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("ConsultantServiceForCountry")]
public partial class ConsultantServiceForCountry
{
    [Key]
    public int Id { get; set; }

    public int ConsultantId { get; set; }

    public int Country { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("ConsultantId")]
    [InverseProperty("ConsultantServiceForCountries")]
    public virtual Consultant Consultant { get; set; } = null!;

    [ForeignKey("Country")]
    [InverseProperty("ConsultantServiceForCountries")]
    public virtual Country CountryNavigation { get; set; } = null!;
}
