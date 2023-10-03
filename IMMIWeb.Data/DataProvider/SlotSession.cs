using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("SlotSession")]
public partial class SlotSession
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? StartHour { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? StartMin { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? EndHour { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? EndMin { get; set; }

    public bool IsAct { get; set; }
}
