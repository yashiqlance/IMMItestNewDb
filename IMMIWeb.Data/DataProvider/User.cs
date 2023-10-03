using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

[Table("User")]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(5)]
    public string? MobileCountryCode { get; set; }

    [StringLength(20)]
    public string? Mobile { get; set; }

    public int? Country { get; set; }

    [StringLength(20)]
    public string DeviceType { get; set; } = null!;

    public string? DeviceToken { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    public bool? IsAgreement { get; set; }

    public bool? IsVerified { get; set; }

    public bool? IsGuest { get; set; }

    public bool? IsRegistered { get; set; }

    public bool IsActive { get; set; }

    [Column("OTP")]
    [StringLength(10)]
    public string? Otp { get; set; }

    public int? LoginAttempt { get; set; }

    public int? CommunicationLanguage { get; set; }

    public int? TypeOfServiceName { get; set; }

    public int? ImmigrationCountry { get; set; }

    public int? ApplicationLanguage { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? OtpDate { get; set; }

    public int UserTypeVal { get; set; }

    [StringLength(100)]
    public string? UserAppleReturnId { get; set; }

    [StringLength(100)]
    public string? UserGoogleReturnId { get; set; }

    public string? ProfilePic { get; set; }

    [StringLength(10)]
    public string? UserSignUpType { get; set; }

    [Column("SocialUID")]
    public string? SocialUid { get; set; }

    public bool? EmailConfirmed { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? EmailVerificationToken { get; set; }

    [Column("CometChatUserUID")]
    [StringLength(200)]
    public string? CometChatUserUid { get; set; }

    public string? AuthenticationToken { get; set; }

    public bool? IsDeleteUserAccount { get; set; }

    [StringLength(50)]
    public string? UniqueId { get; set; }

    [ForeignKey("CommunicationLanguage")]
    [InverseProperty("Users")]
    public virtual Language? CommunicationLanguageNavigation { get; set; }

    [ForeignKey("Country")]
    [InverseProperty("Users")]
    public virtual Country? CountryNavigation { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<FavouriteConsultant> FavouriteConsultants { get; set; } = new List<FavouriteConsultant>();

    [InverseProperty("User")]
    public virtual ICollection<RatingReviewConsultant> RatingReviewConsultants { get; set; } = new List<RatingReviewConsultant>();

    [InverseProperty("User")]
    public virtual ICollection<StripePaymentDetail> StripePaymentDetails { get; set; } = new List<StripePaymentDetail>();

    [ForeignKey("TypeOfServiceName")]
    [InverseProperty("Users")]
    public virtual TypeOfService? TypeOfServiceNameNavigation { get; set; }

    [InverseProperty("IdNavigation")]
    public virtual ICollection<UserCardsDetail> UserCardsDetails { get; set; } = new List<UserCardsDetail>();

    [InverseProperty("User")]
    public virtual ICollection<UserSlotBooking> UserSlotBookings { get; set; } = new List<UserSlotBooking>();

    [ForeignKey("UserTypeVal")]
    [InverseProperty("Users")]
    public virtual UserType UserTypeValNavigation { get; set; } = null!;
}
