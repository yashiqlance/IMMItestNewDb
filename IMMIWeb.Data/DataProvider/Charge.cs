using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

public partial class Charge
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? AppointmentBookingCharges { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? RetainProcessCharges { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? AdminCommissionRate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? ApplicationCharge { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TaxCharge { get; set; }
}
