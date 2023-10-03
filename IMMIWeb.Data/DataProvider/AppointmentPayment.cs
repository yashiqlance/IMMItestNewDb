using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("AppointmentPayment")]
public partial class AppointmentPayment
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ConsultantId { get; set; }

    public int AppointmentId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Amount { get; set; }

    public int PaymentStatusName { get; set; }

    [StringLength(50)]
    public string? StripeCustomerId { get; set; }

    [StringLength(50)]
    public string? TransactionId { get; set; }

    public bool? IsPayment { get; set; }

    public bool? IsAct { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    [StringLength(50)]
    public string? StripeCardId { get; set; }

    public int? CardId { get; set; }

    [ForeignKey("AppointmentId")]
    [InverseProperty("AppointmentPayments")]
    public virtual Appointment Appointment { get; set; } = null!;

    [ForeignKey("PaymentStatusName")]
    [InverseProperty("AppointmentPayments")]
    public virtual PaymentStatus PaymentStatusNameNavigation { get; set; } = null!;
}
