using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("OTPHandler")]
public partial class Otphandler
{
    [Key]
    public int Id { get; set; }

    [Column("GUID")]
    [StringLength(100)]
    public string Guid { get; set; } = null!;

    [Column("OTP")]
    [StringLength(10)]
    public string? Otp { get; set; }
}
