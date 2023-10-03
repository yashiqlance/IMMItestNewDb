using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("AdminInstruction")]
public partial class AdminInstruction
{
    [Key]
    public int Id { get; set; }

    [StringLength(500)]
    public string? Instruction { get; set; }

    public bool? IsAct { get; set; }
}
