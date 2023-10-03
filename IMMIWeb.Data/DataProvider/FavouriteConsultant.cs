using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("FavouriteConsultant")]
public partial class FavouriteConsultant
{
    [Key]
    public int Id { get; set; }

    public int? ConsultantId { get; set; }

    public int? UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    public bool? IsActive { get; set; }

    [ForeignKey("ConsultantId")]
    [InverseProperty("FavouriteConsultants")]
    public virtual Consultant? Consultant { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("FavouriteConsultants")]
    public virtual User? User { get; set; }
}
