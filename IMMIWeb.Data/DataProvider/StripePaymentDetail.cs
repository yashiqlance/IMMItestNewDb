using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("StripePaymentDetail")]
public partial class StripePaymentDetail
{
    [Key]
    public int Id { get; set; }

    public int ConsultantId { get; set; }

    public int UserId { get; set; }

    [StringLength(100)]
    public string TokenId { get; set; } = null!;

    [StringLength(100)]
    public string CustomerId { get; set; } = null!;

    [StringLength(100)]
    public string AppointmentfeesChargeId { get; set; } = null!;

    public int CardId { get; set; }

    public bool? IsActive { get; set; }

    [StringLength(100)]
    public string RetentionfeesChargeId { get; set; } = null!;

    [StringLength(100)]
    public string CancelationChargeId { get; set; } = null!;

    [StringLength(100)]
    public string StripeCardId { get; set; } = null!;

    [Column("ConsultantStripeAccountID")]
    [StringLength(100)]
    public string ConsultantStripeAccountId { get; set; } = null!;

    [ForeignKey("CardId")]
    [InverseProperty("StripePaymentDetails")]
    public virtual UserCardsDetail Card { get; set; } = null!;

    [ForeignKey("ConsultantId")]
    [InverseProperty("StripePaymentDetails")]
    public virtual Consultant Consultant { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("StripePaymentDetails")]
    public virtual User User { get; set; } = null!;
}
