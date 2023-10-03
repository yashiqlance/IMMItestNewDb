using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("UserType")]
public partial class UserType
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [InverseProperty("CancelledByUserTypeNameNavigation")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [InverseProperty("UserTypeValNavigation")]
    public virtual ICollection<Consultant> Consultants { get; set; } = new List<Consultant>();

    [InverseProperty("UserTypeValNavigation")]
    public virtual ICollection<CrmUser> CrmUsers { get; set; } = new List<CrmUser>();

    [InverseProperty("UserTypeValNavigation")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
