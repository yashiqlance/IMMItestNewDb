using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("NotificationMaster")]
public partial class NotificationMaster
{
    [Key]
    public int Id { get; set; }

    public int NotificationTypeName { get; set; }

    [StringLength(50)]
    public string Header { get; set; } = null!;

    [StringLength(500)]
    public string Body { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public int? SenderUserType { get; set; }

    public int? ReceiverUserType { get; set; }

    public bool? IsAct { get; set; }

    public bool? IsRead { get; set; }

    [StringLength(50)]
    public string? Baner { get; set; }
}
