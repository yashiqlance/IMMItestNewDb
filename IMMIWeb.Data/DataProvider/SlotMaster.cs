using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("SlotMaster")]
public partial class SlotMaster
{
    [Key]
    public int Id { get; set; }

    public double StartTime { get; set; }

    public double EndTime { get; set; }

    public bool IsActive { get; set; }

    [InverseProperty("TimeNavigation")]
    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();
}
