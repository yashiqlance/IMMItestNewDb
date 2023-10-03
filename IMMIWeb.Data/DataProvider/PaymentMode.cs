using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("PaymentMode")]
public partial class PaymentMode
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Title { get; set; } = null!;

    [InverseProperty("PaymentModeNameNavigation")]
    public virtual ICollection<RetainPayment> RetainPayments { get; set; } = new List<RetainPayment>();
}
