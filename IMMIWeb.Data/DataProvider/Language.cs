using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("Language")]
public partial class Language
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    [InverseProperty("LanguageNavigation")]
    public virtual ICollection<ConsultantLanguage> ConsultantLanguages { get; set; } = new List<ConsultantLanguage>();

    [InverseProperty("CommunicationLanguageNavigation")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
