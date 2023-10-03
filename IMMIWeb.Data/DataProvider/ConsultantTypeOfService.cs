using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("ConsultantTypeOfService")]
public partial class ConsultantTypeOfService
{
    [Key]
    public int Id { get; set; }

    public int ConsultantId { get; set; }

    public int TypeOfService { get; set; }

    public bool IsActive { get; set; }

    public int? CountryId { get; set; }

    [ForeignKey("ConsultantId")]
    [InverseProperty("ConsultantTypeOfServices")]
    public virtual Consultant Consultant { get; set; } = null!;

    [ForeignKey("TypeOfService")]
    [InverseProperty("ConsultantTypeOfServices")]
    public virtual TypeOfService TypeOfServiceNavigation { get; set; } = null!;
}
