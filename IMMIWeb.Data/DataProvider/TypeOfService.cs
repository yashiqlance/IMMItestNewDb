using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("TypeOfService")]
public partial class TypeOfService
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public int? CountryId { get; set; }

    [InverseProperty("TypeOfServiceNavigation")]
    public virtual ICollection<ConsultantTypeOfService> ConsultantTypeOfServices { get; set; } = new List<ConsultantTypeOfService>();

    [InverseProperty("TypeOfServiceNameNavigation")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
