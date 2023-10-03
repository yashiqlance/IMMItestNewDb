using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("Social")]
public partial class Social
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int SocialTypeOfService { get; set; }

    public int SocialCountryForService { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    public bool? IsAct { get; set; }

    [InverseProperty("Social")]
    public virtual ICollection<SocialResponse> SocialResponses { get; set; } = new List<SocialResponse>();
}
