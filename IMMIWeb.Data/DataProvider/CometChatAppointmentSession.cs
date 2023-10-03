using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("CometChatAppointmentSession")]
public partial class CometChatAppointmentSession
{
    [Key]
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    public int CometChatSessionId { get; set; }
}
