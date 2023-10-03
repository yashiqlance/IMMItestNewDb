using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("Withdraw")]
public partial class Withdraw
{
    [Key]
    public int Id { get; set; }

    public int ConsultantId { get; set; }

    [Column("ConsultantStripeAccountID")]
    [StringLength(100)]
    public string ConsultantStripeAccountId { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal WithdrawAmount { get; set; }

    [StringLength(50)]
    public string? StripeTransferId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    [StringLength(20)]
    public string? WithdrawStatus { get; set; }

    public bool? IsActive { get; set; }

    [ForeignKey("ConsultantId")]
    [InverseProperty("Withdraws")]
    public virtual Consultant Consultant { get; set; } = null!;
}
