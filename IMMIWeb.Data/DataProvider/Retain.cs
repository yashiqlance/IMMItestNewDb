using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("Retain")]
public partial class Retain
{
    [Key]
    public int Id { get; set; }

    public int ConsultantId { get; set; }

    public int UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    public bool IsAct { get; set; }

    public int RetainTypeOfService { get; set; }

    public int RetainCountryForService { get; set; }

    public int RetainCommunicationLanguage { get; set; }

    [InverseProperty("Retain")]
    public virtual ICollection<Emitable> Emitables { get; set; } = new List<Emitable>();

    [InverseProperty("Retain")]
    public virtual ICollection<RetainPayment> RetainPayments { get; set; } = new List<RetainPayment>();
}
