using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("RatingReviewConsultant")]
public partial class RatingReviewConsultant
{
    [Key]
    public int Id { get; set; }

    public int? ConsultantId { get; set; }

    public int? UserId { get; set; }

    public int? Rating { get; set; }

    public string? Review { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    public bool? IsActive { get; set; }

    [ForeignKey("ConsultantId")]
    [InverseProperty("RatingReviewConsultants")]
    public virtual Consultant? Consultant { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("RatingReviewConsultants")]
    public virtual User? User { get; set; }
}
