using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("UserRetainToConsultant")]
public partial class UserRetainToConsultant
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ConsultantId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    public bool IsAct { get; set; }
}
