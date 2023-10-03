using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("Appointment")]
public partial class Appointment
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? ConsultantSlotId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    public int? UserRequestTypeName { get; set; }

    public int? SessionMode { get; set; }

    public bool? IsConsultantPresent { get; set; }

    public bool? IsUserPresent { get; set; }

    public int? AppointmentStatusName { get; set; }

    public int? CancelledByUserTypeName { get; set; }

    public int? CancelledById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CancellationDate { get; set; }

    [StringLength(500)]
    public string? CancellationReason { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? AppointmentDate { get; set; }

    [StringLength(200)]
    public string? SlotSessionId { get; set; }

    public int? ServiceType { get; set; }

    public int? ApplyForCountry { get; set; }

    public int? ConsultantId { get; set; }

    public bool? IsCancel { get; set; }

    public bool? IsCompleted { get; set; }

    public bool? IsUpcoming { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastUpdatedOn { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? RejectionAmount { get; set; }

    [Column("CUID")]
    [StringLength(200)]
    public string? Cuid { get; set; }

    public int? ExtendCount { get; set; }

    public bool? IsJobDone { get; set; }

    [InverseProperty("Appointment")]
    public virtual ICollection<AppointmentPayment> AppointmentPayments { get; set; } = new List<AppointmentPayment>();

    [ForeignKey("AppointmentStatusName")]
    [InverseProperty("Appointments")]
    public virtual AppointmentStatus? AppointmentStatusNameNavigation { get; set; }

    [ForeignKey("CancelledByUserTypeName")]
    [InverseProperty("Appointments")]
    public virtual UserType? CancelledByUserTypeNameNavigation { get; set; }

    [ForeignKey("SessionMode")]
    [InverseProperty("Appointments")]
    public virtual UserRequestSessionType? SessionModeNavigation { get; set; }

    [ForeignKey("UserRequestTypeName")]
    [InverseProperty("Appointments")]
    public virtual UserRequestType? UserRequestTypeNameNavigation { get; set; }
}
