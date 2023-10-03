using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("ConsultantChargeRequest")]
public partial class ConsultantChargeRequest
{
    [Key]
    public int Id { get; set; }

    public int ConsultantId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Amount { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("ConsultantId")]
    [InverseProperty("ConsultantChargeRequests")]
    public virtual Consultant Consultant { get; set; } = null!;
}
