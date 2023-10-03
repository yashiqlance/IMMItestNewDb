using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("AdminNotification")]
public partial class AdminNotification
{
    [Key]
    public int Id { get; set; }

    [StringLength(300)]
    public string Title { get; set; } = null!;

    [StringLength(1000)]
    public string Description { get; set; } = null!;

    [StringLength(50)]
    public string Role { get; set; } = null!;

    public int? SpecificRole { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    [StringLength(50)]
    public string? Baner { get; set; }

    public bool IsActive { get; set; }

    public bool? IsRead { get; set; }
}
