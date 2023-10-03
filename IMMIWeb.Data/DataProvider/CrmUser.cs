using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("CrmUser")]
public partial class CrmUser
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Email { get; set; } = null!;

    [StringLength(50)]
    public string Password { get; set; } = null!;

    public int UserTypeVal { get; set; }

    [ForeignKey("UserTypeVal")]
    [InverseProperty("CrmUsers")]
    public virtual UserType UserTypeValNavigation { get; set; } = null!;
}
