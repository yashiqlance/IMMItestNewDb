using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

public partial class CometChatLog
{
    [Key]
    public int Id { get; set; }

    public int? AppointmentId { get; set; }

    public int? UserId { get; set; }

    public int? ConsultantId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    public bool? IsActive { get; set; }

    [Column("VideoURL")]
    [StringLength(500)]
    public string? VideoUrl { get; set; }

    [Column("AudioURL")]
    [StringLength(500)]
    public string? AudioUrl { get; set; }

    [Column("TextURL")]
    [StringLength(500)]
    public string? TextUrl { get; set; }

    [Column("FileURL")]
    [StringLength(500)]
    public string? FileUrl { get; set; }

    [Column("SessionID")]
    [StringLength(500)]
    public string? SessionId { get; set; }

    public bool? IsCallExtended { get; set; }

    public int? CallExtendedCount { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? SessionStartTime { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? SessionEndTime { get; set; }

    [StringLength(50)]
    public string? SessionStartBy { get; set; }

    [StringLength(50)]
    public string? SessionEndBy { get; set; }

    public bool? IsAnyTechnicalIssue { get; set; }
}
