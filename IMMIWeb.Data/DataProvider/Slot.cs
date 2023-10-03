using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("Slot")]
public partial class Slot
{
    [Key]
    public int Id { get; set; }

    public int ConsultantId { get; set; }

    public int DayName { get; set; }

    public int Time { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    [StringLength(10)]
    public string IsActive { get; set; } = null!;

    [ForeignKey("ConsultantId")]
    [InverseProperty("Slots")]
    public virtual Consultant Consultant { get; set; } = null!;

    [ForeignKey("DayName")]
    [InverseProperty("Slots")]
    public virtual DayMaster DayNameNavigation { get; set; } = null!;

    [ForeignKey("Time")]
    [InverseProperty("Slots")]
    public virtual SlotMaster TimeNavigation { get; set; } = null!;

    [InverseProperty("Slot")]
    public virtual ICollection<UserSlotBooking> UserSlotBookings { get; set; } = new List<UserSlotBooking>();
}
