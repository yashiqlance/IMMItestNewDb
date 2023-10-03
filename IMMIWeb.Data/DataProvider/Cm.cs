using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("CMS")]
public partial class Cm
{
    [Key]
    public int Id { get; set; }

    [StringLength(2000)]
    public string Description { get; set; } = null!;

    public int Module { get; set; }

    public bool IsActive { get; set; }

    public int UserRole { get; set; }
}
