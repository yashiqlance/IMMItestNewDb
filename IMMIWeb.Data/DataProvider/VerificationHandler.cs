using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("VerificationHandler")]
public partial class VerificationHandler
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string UniqueId { get; set; } = null!;

    [StringLength(50)]
    public string Platform { get; set; } = null!;

    [StringLength(50)]
    public string PlatformDetail { get; set; } = null!;

    public int Attempt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    public bool IsActive { get; set; }
}
