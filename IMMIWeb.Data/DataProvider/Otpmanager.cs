using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Keyless]
[Table("OTPManager")]
public partial class Otpmanager
{
    [Column("GUID")]
    [StringLength(50)]
    public string Guid { get; set; } = null!;

    [Column("OTP")]
    [StringLength(10)]
    public string Otp { get; set; } = null!;
}
