using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("UserRequestType")]
public partial class UserRequestType
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string RequestTitle { get; set; } = null!;

    [InverseProperty("UserRequestTypeNameNavigation")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
