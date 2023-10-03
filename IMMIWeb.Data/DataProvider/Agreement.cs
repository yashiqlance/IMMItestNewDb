using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("Agreement")]
public partial class Agreement
{
    [Key]
    public int Id { get; set; }

    [StringLength(2000)]
    public string Description { get; set; } = null!;

    public int Type { get; set; }

    public bool IsActive { get; set; }
}
