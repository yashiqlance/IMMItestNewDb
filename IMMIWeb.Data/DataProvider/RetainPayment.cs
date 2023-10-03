using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("RetainPayment")]
public partial class RetainPayment
{
    [Key]
    public int Id { get; set; }

    public int RetainId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? RetainAmount { get; set; }

    public int PaymentModeName { get; set; }

    [StringLength(50)]
    public string? RetainStripeAccountId { get; set; }

    [StringLength(50)]
    public string? TransactionId { get; set; }

    public int? ForNumberOfEmi { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    public int? RetainPaymentStatus { get; set; }

    public bool? IsRetainPaymentPaid { get; set; }

    [ForeignKey("PaymentModeName")]
    [InverseProperty("RetainPayments")]
    public virtual PaymentMode PaymentModeNameNavigation { get; set; } = null!;

    [ForeignKey("RetainId")]
    [InverseProperty("RetainPayments")]
    public virtual Retain Retain { get; set; } = null!;

    [ForeignKey("RetainPaymentStatus")]
    [InverseProperty("RetainPayments")]
    public virtual PaymentStatus? RetainPaymentStatusNavigation { get; set; }
}
