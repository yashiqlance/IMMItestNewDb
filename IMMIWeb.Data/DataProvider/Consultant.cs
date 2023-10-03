using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("Consultant")]
public partial class Consultant
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string LicenceNumber { get; set; } = null!;

    [StringLength(5)]
    public string MobileCountryCode { get; set; } = null!;

    [StringLength(20)]
    public string? Mobile { get; set; }

    public int? Country { get; set; }

    [StringLength(50)]
    public string? DeviceType { get; set; }

    [StringLength(500)]
    public string? DeviceToken { get; set; }

    public bool IsAgreement { get; set; }

    public bool? IsVerified { get; set; }

    public bool? IsAdminApproved { get; set; }

    public bool IsActive { get; set; }

    public bool IsSuspended { get; set; }

    [StringLength(10)]
    public string? Currency { get; set; }

    public int? LoginAttempt { get; set; }

    [Column("OTP")]
    [StringLength(10)]
    public string? Otp { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? OtpDate { get; set; }

    public int? ApplicationLanguage { get; set; }

    public int UserTypeVal { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    public bool? IsAvailable { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? RetainAmount { get; set; }

    [StringLength(100)]
    public string? UserAppleReturnId { get; set; }

    [StringLength(100)]
    public string? UserGoogleReturnId { get; set; }

    [StringLength(50)]
    public string? ConsultantStripeAccountId { get; set; }

    [StringLength(200)]
    public string? ProfilePic { get; set; }

    public bool? EmailConfirmed { get; set; }

    public bool? IsConsultantStripAccountVerified { get; set; }

    [StringLength(10)]
    public string? EmailVerificationToken { get; set; }

    [StringLength(10)]
    public string? ConsultantSignUpType { get; set; }

    [Column("CometChatConsultantUID")]
    [StringLength(200)]
    public string? CometChatConsultantUid { get; set; }

    public string? AuthenticationToken { get; set; }

    public bool? IsDeleteConsultantAccount { get; set; }

    public int? RequestRejectionCount { get; set; }

    [StringLength(30)]
    public string? SuspendedBy { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? EarnAmount { get; set; }

    [StringLength(50)]
    public string? UniqueId { get; set; }

    [Column("ISAdminTransfer")]
    public bool? IsadminTransfer { get; set; }

    [InverseProperty("Consultant")]
    public virtual ICollection<ConsultantChargeRequest> ConsultantChargeRequests { get; set; } = new List<ConsultantChargeRequest>();

    [InverseProperty("Consultant")]
    public virtual ICollection<ConsultantLanguage> ConsultantLanguages { get; set; } = new List<ConsultantLanguage>();

    [InverseProperty("Consultant")]
    public virtual ICollection<ConsultantServiceForCountry> ConsultantServiceForCountries { get; set; } = new List<ConsultantServiceForCountry>();

    [InverseProperty("Consultant")]
    public virtual ICollection<ConsultantTypeOfService> ConsultantTypeOfServices { get; set; } = new List<ConsultantTypeOfService>();

    [InverseProperty("Consultant")]
    public virtual ICollection<FavouriteConsultant> FavouriteConsultants { get; set; } = new List<FavouriteConsultant>();

    [InverseProperty("Consultant")]
    public virtual ICollection<RatingReviewConsultant> RatingReviewConsultants { get; set; } = new List<RatingReviewConsultant>();

    [InverseProperty("Consultant")]
    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();

    [InverseProperty("Consultant")]
    public virtual ICollection<StripePaymentDetail> StripePaymentDetails { get; set; } = new List<StripePaymentDetail>();

    [InverseProperty("Consultant")]
    public virtual ICollection<UserSlotBooking> UserSlotBookings { get; set; } = new List<UserSlotBooking>();

    [ForeignKey("UserTypeVal")]
    [InverseProperty("Consultants")]
    public virtual UserType UserTypeValNavigation { get; set; } = null!;

    [InverseProperty("Consultant")]
    public virtual ICollection<Withdraw> Withdraws { get; set; } = new List<Withdraw>();
}
