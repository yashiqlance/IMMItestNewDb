using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("PaymentStatus")]
public partial class PaymentStatus
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string PaymentTitle { get; set; } = null!;

    public bool IsAct { get; set; }

    [InverseProperty("PaymentStatusNameNavigation")]
    public virtual ICollection<AppointmentPayment> AppointmentPayments { get; set; } = new List<AppointmentPayment>();

    [InverseProperty("EmiPaymentStatusNavigation")]
    public virtual ICollection<Emitable> Emitables { get; set; } = new List<Emitable>();

    [InverseProperty("RetainPaymentStatusNavigation")]
    public virtual ICollection<RetainPayment> RetainPayments { get; set; } = new List<RetainPayment>();
}
