using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

public partial class ContactU
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string EmailId { get; set; } = null!;

    [StringLength(100)]
    public string Subject { get; set; } = null!;

    [StringLength(2000)]
    public string Description { get; set; } = null!;

    public bool IsActive { get; set; }
}
