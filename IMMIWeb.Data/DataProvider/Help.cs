using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("Help")]
public partial class Help
{
    [Key]
    public int Id { get; set; }

    public string? Question { get; set; }

    public string? Answer { get; set; }
}
