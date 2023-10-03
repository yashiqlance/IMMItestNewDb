using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("UserDocument")]
public partial class UserDocument
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    [StringLength(100)]
    public string Filename { get; set; } = null!;

    [MaxLength(500)]
    public byte[]? Size { get; set; }

    [StringLength(10)]
    public string Extensions { get; set; } = null!;

    public int? RetainId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    public bool IsActive { get; set; }

    [Column("DocURL")]
    public string? DocUrl { get; set; }
}
