using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("AppointmentStatus")]
public partial class AppointmentStatus
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string AppointmentTitle { get; set; } = null!;

    [InverseProperty("AppointmentStatusNameNavigation")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
