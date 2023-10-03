using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("ConsultantLanguage")]
public partial class ConsultantLanguage
{
    [Key]
    public int Id { get; set; }

    public int ConsultantId { get; set; }

    public int Language { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("ConsultantId")]
    [InverseProperty("ConsultantLanguages")]
    public virtual Consultant Consultant { get; set; } = null!;

    [ForeignKey("Language")]
    [InverseProperty("ConsultantLanguages")]
    public virtual Language LanguageNavigation { get; set; } = null!;
}
