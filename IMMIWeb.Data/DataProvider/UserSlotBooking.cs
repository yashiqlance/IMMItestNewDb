using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("UserSlotBooking")]
public partial class UserSlotBooking
{
    [Key]
    public int Id { get; set; }

    public int ConsultantId { get; set; }

    public int UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Date { get; set; }

    public int Slotid { get; set; }

    public bool IsActive { get; set; }

    [StringLength(20)]
    public string CommunicationMode { get; set; } = null!;

    public int? ExtendFlag { get; set; }

    public int? IsConsultantStatus { get; set; }

    public int? IsUserStatus { get; set; }

    [ForeignKey("ConsultantId")]
    [InverseProperty("UserSlotBookings")]
    public virtual Consultant Consultant { get; set; } = null!;

    [ForeignKey("Slotid")]
    [InverseProperty("UserSlotBookings")]
    public virtual Slot Slot { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserSlotBookings")]
    public virtual User User { get; set; } = null!;
}
