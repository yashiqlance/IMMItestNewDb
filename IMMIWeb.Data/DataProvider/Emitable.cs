using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("EMITable")]
public partial class Emitable
{
    [Key]
    public int Id { get; set; }

    public int RetainId { get; set; }

    [StringLength(100)]
    public string StripeCustomerId { get; set; } = null!;

    [StringLength(100)]
    public string StripeCardId { get; set; } = null!;

    public int? ForNumberOfEmi { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal PaidAmount { get; set; }

    [Column("EMICount")]
    public int? Emicount { get; set; }

    public bool? IsActive { get; set; }

    [Column("EMIAmount", TypeName = "decimal(18, 2)")]
    public decimal Emiamount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    public int? EmiPaymentStatus { get; set; }

    public bool? IsEmiPaymentPaid { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EmiDate { get; set; }

    [ForeignKey("EmiPaymentStatus")]
    [InverseProperty("Emitables")]
    public virtual PaymentStatus? EmiPaymentStatusNavigation { get; set; }

    [ForeignKey("RetainId")]
    [InverseProperty("Emitables")]
    public virtual Retain Retain { get; set; } = null!;
}
