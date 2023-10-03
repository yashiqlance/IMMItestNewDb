using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("ConsultantSlot")]
public partial class ConsultantSlot
{
    [Key]
    public int Id { get; set; }

    public int ConsultantId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Date { get; set; }

    public bool? IsActive { get; set; }

    public int StartHour { get; set; }

    public int EndHour { get; set; }

    public string? TimeZone { get; set; }
}
