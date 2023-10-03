using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("UserRequestSessionType")]
public partial class UserRequestSessionType
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string SessionTitle { get; set; } = null!;

    [InverseProperty("SessionModeNavigation")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
