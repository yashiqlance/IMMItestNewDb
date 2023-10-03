using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("DayMaster")]
public partial class DayMaster
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    [InverseProperty("DayNameNavigation")]
    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();
}
