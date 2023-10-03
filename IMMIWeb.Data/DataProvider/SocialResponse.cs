using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("SocialResponse")]
public partial class SocialResponse
{
    [Key]
    public int Id { get; set; }

    public int SocialId { get; set; }

    public int ConsultantId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    public bool? IsAct { get; set; }

    [ForeignKey("SocialId")]
    [InverseProperty("SocialResponses")]
    public virtual Social Social { get; set; } = null!;
}
