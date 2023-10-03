using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb;

public partial class DbA976eeImmidevContext : DbContext
{
    public DbA976eeImmidevContext()
    {
    }

    public DbA976eeImmidevContext(DbContextOptions<DbA976eeImmidevContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminInstruction> AdminInstructions { get; set; }

    public virtual DbSet<AdminNotification> AdminNotifications { get; set; }

    public virtual DbSet<AppLanguage> AppLanguages { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentPayment> AppointmentPayments { get; set; }

    public virtual DbSet<AppointmentStatus> AppointmentStatuses { get; set; }

    public virtual DbSet<Charge> Charges { get; set; }

    public virtual DbSet<Cm> Cms { get; set; }

    public virtual DbSet<CometChatAppointmentSession> CometChatAppointmentSessions { get; set; }

    public virtual DbSet<CometChatLog> CometChatLogs { get; set; }

    public virtual DbSet<Consultant> Consultants { get; set; }

    public virtual DbSet<ConsultantChargeRequest> ConsultantChargeRequests { get; set; }

    public virtual DbSet<ConsultantLanguage> ConsultantLanguages { get; set; }

    public virtual DbSet<ConsultantServiceForCountry> ConsultantServiceForCountries { get; set; }

    public virtual DbSet<ConsultantSlot> ConsultantSlots { get; set; }

    public virtual DbSet<ConsultantTypeOfService> ConsultantTypeOfServices { get; set; }

    public virtual DbSet<ContactU> ContactUs { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<CrmUser> CrmUsers { get; set; }

    public virtual DbSet<DayMaster> DayMasters { get; set; }

    public virtual DbSet<Emitable> Emitables { get; set; }

    public virtual DbSet<FavouriteConsultant> FavouriteConsultants { get; set; }

    public virtual DbSet<Help> Helps { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<MyUser> MyUsers { get; set; }

    public virtual DbSet<NotificationMaster> NotificationMasters { get; set; }

    public virtual DbSet<NotificationType> NotificationTypes { get; set; }

    public virtual DbSet<Otphandler> Otphandlers { get; set; }

    public virtual DbSet<PaymentGeneral> PaymentGenerals { get; set; }

    public virtual DbSet<PaymentMode> PaymentModes { get; set; }

    public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }

    public virtual DbSet<RatingReviewConsultant> RatingReviewConsultants { get; set; }

    public virtual DbSet<Retain> Retains { get; set; }

    public virtual DbSet<RetainPayment> RetainPayments { get; set; }

    public virtual DbSet<Slot> Slots { get; set; }

    public virtual DbSet<SlotMaster> SlotMasters { get; set; }

    public virtual DbSet<SlotSession> SlotSessions { get; set; }

    public virtual DbSet<Social> Socials { get; set; }

    public virtual DbSet<SocialResponse> SocialResponses { get; set; }

    public virtual DbSet<StripePaymentDetail> StripePaymentDetails { get; set; }

    public virtual DbSet<TypeOfService> TypeOfServices { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserCardsDetail> UserCardsDetails { get; set; }

    public virtual DbSet<UserDocument> UserDocuments { get; set; }

    public virtual DbSet<UserRequestSessionType> UserRequestSessionTypes { get; set; }

    public virtual DbSet<UserRequestType> UserRequestTypes { get; set; }

    public virtual DbSet<UserRetainToConsultant> UserRetainToConsultants { get; set; }

    public virtual DbSet<UserSlotBooking> UserSlotBookings { get; set; }

    public virtual DbSet<UserType> UserTypes { get; set; }

    public virtual DbSet<VerificationHandler> VerificationHandlers { get; set; }

    public virtual DbSet<Withdraw> Withdraws { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=SQL8005.site4now.net;user=db_a976ee_immidev_admin;password=Immi@123;database=db_a976ee_immidev;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppLanguage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AppLangu__3214EC0788C548CC");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasOne(d => d.AppointmentStatusNameNavigation).WithMany(p => p.Appointments).HasConstraintName("FK_Appointment_AppointmentStatus");

            entity.HasOne(d => d.CancelledByUserTypeNameNavigation).WithMany(p => p.Appointments).HasConstraintName("FK_Appointment_UserType");

            entity.HasOne(d => d.SessionModeNavigation).WithMany(p => p.Appointments).HasConstraintName("FK_Appointment_UserRequestSessionType");

            entity.HasOne(d => d.UserRequestTypeNameNavigation).WithMany(p => p.Appointments).HasConstraintName("FK_Appointment_UserRequestType");
        });

        modelBuilder.Entity<AppointmentPayment>(entity =>
        {
            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentPayments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AppointmentPayment_Appointment");

            entity.HasOne(d => d.PaymentStatusNameNavigation).WithMany(p => p.AppointmentPayments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AppointmentPayment_PaymentStatus");
        });

        modelBuilder.Entity<Cm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CMS__3214EC07360EF309");
        });

        modelBuilder.Entity<Consultant>(entity =>
        {
            entity.HasOne(d => d.UserTypeValNavigation).WithMany(p => p.Consultants)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consultant_UserType");
        });

        modelBuilder.Entity<ConsultantChargeRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Consulta__3214EC071295430B");

            entity.HasOne(d => d.Consultant).WithMany(p => p.ConsultantChargeRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Consultan__Consu__7B264821");
        });

        modelBuilder.Entity<ConsultantLanguage>(entity =>
        {
            entity.HasOne(d => d.Consultant).WithMany(p => p.ConsultantLanguages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConsultantLanguage_Consultant");

            entity.HasOne(d => d.LanguageNavigation).WithMany(p => p.ConsultantLanguages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConsultantLanguage_Language");
        });

        modelBuilder.Entity<ConsultantServiceForCountry>(entity =>
        {
            entity.HasOne(d => d.Consultant).WithMany(p => p.ConsultantServiceForCountries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConsultantServiceForCountry_Consultant");

            entity.HasOne(d => d.CountryNavigation).WithMany(p => p.ConsultantServiceForCountries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConsultantServiceForCountry_Country");
        });

        modelBuilder.Entity<ConsultantTypeOfService>(entity =>
        {
            entity.HasOne(d => d.Consultant).WithMany(p => p.ConsultantTypeOfServices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConsultantTypeOfService_Consultant");

            entity.HasOne(d => d.TypeOfServiceNavigation).WithMany(p => p.ConsultantTypeOfServices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConsultantTypeOfService_TypeOfService");
        });

        modelBuilder.Entity<CrmUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Crmuser");

            entity.HasOne(d => d.UserTypeValNavigation).WithMany(p => p.CrmUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CrmUser_UserType");
        });

        modelBuilder.Entity<Emitable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EMITable__3214EC07AEECB9B9");

            entity.HasOne(d => d.EmiPaymentStatusNavigation).WithMany(p => p.Emitables).HasConstraintName("FK__EMITable__EmiPay__4E1E9780");

            entity.HasOne(d => d.Retain).WithMany(p => p.Emitables)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EMITable__Retain__22401542");
        });

        modelBuilder.Entity<FavouriteConsultant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Favourit__3214EC07507F10F1");

            entity.HasOne(d => d.Consultant).WithMany(p => p.FavouriteConsultants).HasConstraintName("FK__Favourite__Consu__0A9D95DB");

            entity.HasOne(d => d.User).WithMany(p => p.FavouriteConsultants).HasConstraintName("FK__Favourite__UserI__0B91BA14");
        });

        modelBuilder.Entity<Help>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Help__3214EC073D718C72");
        });

        modelBuilder.Entity<PaymentGeneral>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentG__3214EC07ADF8A3EE");
        });

        modelBuilder.Entity<RatingReviewConsultant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RatingRe__3214EC07B00EF389");

            entity.HasOne(d => d.Consultant).WithMany(p => p.RatingReviewConsultants).HasConstraintName("FK__RatingRev__Consu__0E6E26BF");

            entity.HasOne(d => d.User).WithMany(p => p.RatingReviewConsultants).HasConstraintName("FK__RatingRev__UserI__0F624AF8");
        });

        modelBuilder.Entity<RetainPayment>(entity =>
        {
            entity.HasOne(d => d.PaymentModeNameNavigation).WithMany(p => p.RetainPayments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RetainPayment_PaymentMode");

            entity.HasOne(d => d.Retain).WithMany(p => p.RetainPayments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RetainPayment_Retain");

            entity.HasOne(d => d.RetainPaymentStatusNavigation).WithMany(p => p.RetainPayments).HasConstraintName("FK__RetainPay__Retai__4D2A7347");
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            entity.Property(e => e.IsActive).IsFixedLength();

            entity.HasOne(d => d.Consultant).WithMany(p => p.Slots)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Slot_Consultant");

            entity.HasOne(d => d.DayNameNavigation).WithMany(p => p.Slots)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Slot_DayMaster");

            entity.HasOne(d => d.TimeNavigation).WithMany(p => p.Slots)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Slot_SlotMaster");
        });

        modelBuilder.Entity<SocialResponse>(entity =>
        {
            entity.HasOne(d => d.Social).WithMany(p => p.SocialResponses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocialResponse_Social");
        });

        modelBuilder.Entity<StripePaymentDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StripePa__3214EC07C7ED81C7");

            entity.HasOne(d => d.Card).WithMany(p => p.StripePaymentDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StripePay__CardI__55009F39");

            entity.HasOne(d => d.Consultant).WithMany(p => p.StripePaymentDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StripePay__Consu__531856C7");

            entity.HasOne(d => d.User).WithMany(p => p.StripePaymentDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StripePay__UserI__540C7B00");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_User_1");

            entity.HasOne(d => d.CommunicationLanguageNavigation).WithMany(p => p.Users).HasConstraintName("FK_User_Language");

            entity.HasOne(d => d.CountryNavigation).WithMany(p => p.Users).HasConstraintName("FK_User_Country");

            entity.HasOne(d => d.TypeOfServiceNameNavigation).WithMany(p => p.Users).HasConstraintName("FK_User_TypeOfService");

            entity.HasOne(d => d.UserTypeValNavigation).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_UserType");
        });

        modelBuilder.Entity<UserCardsDetail>(entity =>
        {
            entity.HasKey(e => e.CardId).HasName("PK__UserCard__55FECDAEE99A986C");

            entity.HasOne(d => d.IdNavigation).WithMany(p => p.UserCardsDetails).HasConstraintName("FK__UserCardsDet__Id__73BA3083");
        });

        modelBuilder.Entity<UserSlotBooking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserSlot__3214EC076CD2FC55");

            entity.HasOne(d => d.Consultant).WithMany(p => p.UserSlotBookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserSlotBooking_Consultant");

            entity.HasOne(d => d.Slot).WithMany(p => p.UserSlotBookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserSlotBooking_Slot");

            entity.HasOne(d => d.User).WithMany(p => p.UserSlotBookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserSlotBooking_User");
        });

        modelBuilder.Entity<Withdraw>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Withdraw__3214EC07FD6B0E4B");

            entity.HasOne(d => d.Consultant).WithMany(p => p.Withdraws)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Withdraw__Consul__13F1F5EB");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
