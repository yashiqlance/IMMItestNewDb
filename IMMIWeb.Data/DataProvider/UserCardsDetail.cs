using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("UserCardsDetail")]
public partial class UserCardsDetail
{
    [Key]
    public int CardId { get; set; }

    [StringLength(100)]
    public string CardName { get; set; } = null!;

    [StringLength(16)]
    [Unicode(false)]
    public string CardNumber { get; set; } = null!;

    [Column("CVV")]
    public int Cvv { get; set; }

    public int? Id { get; set; }

    [StringLength(50)]
    public string StripeCustomerId { get; set; } = null!;

    [StringLength(50)]
    public string StripeCardId { get; set; } = null!;

    public bool? IsPrimary { get; set; }

    [StringLength(20)]
    public string? ExpMonth { get; set; }

    [StringLength(20)]
    public string? ExpYear { get; set; }

    [ForeignKey("Id")]
    [InverseProperty("UserCardsDetails")]
    public virtual User? IdNavigation { get; set; }

    [InverseProperty("Card")]
    public virtual ICollection<StripePaymentDetail> StripePaymentDetails { get; set; } = new List<StripePaymentDetail>();
}
