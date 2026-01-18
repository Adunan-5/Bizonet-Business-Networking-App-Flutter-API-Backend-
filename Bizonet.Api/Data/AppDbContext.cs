using Bizonet.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bizonet.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserOtp> UserOtps => Set<UserOtp>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<State> States => Set<State>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<BusinessCategory> BusinessCategories => Set<BusinessCategory>();
    public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();
    public DbSet<BusinessInfo> BusinessInfos => Set<BusinessInfo>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupUser> GroupUsers => Set<GroupUser>();
    public DbSet<FollowupStatus> FollowupStatuses => Set<FollowupStatus>();
    public DbSet<FollowupComment> FollowupComments => Set<FollowupComment>();
    public DbSet<Referral> Referrals => Set<Referral>();
    public DbSet<ReferralOutsider> ReferralOutsiders => Set<ReferralOutsider>();
    public DbSet<ReferralStatus> ReferralStatuses => Set<ReferralStatus>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.UserId);

            entity.Property(x => x.ProfilePhoto)
                  .HasDefaultValue("defultprofilephoto.jpg");

            entity.Property(x => x.Active)
                  .HasDefaultValue(true);

            entity.Property(x => x.UserRole)
                  .HasDefaultValue("user");

            entity.HasIndex(x => x.Email)
                  .IsUnique()
                  .HasFilter("[Email] IS NOT NULL");

            entity.HasIndex(x => x.MobileNo)
                  .IsUnique()
                  .HasFilter("[MobileNo] IS NOT NULL");
        });

        modelBuilder.Entity<UserOtp>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.User)
                  .WithMany(x => x.Otps)
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(x => x.CountryId);

            entity.Property(x => x.CountryCode).HasMaxLength(3);

            entity.Property(x => x.So).HasDefaultValue((byte)0);
            entity.Property(x => x.Active).HasDefaultValue((byte)1);
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(x => x.StateId);

            entity.Property(x => x.CountryId).HasDefaultValue(1);

            entity.Property(x => x.So).HasDefaultValue((byte)0);
            entity.Property(x => x.Active).HasDefaultValue((byte)1);

            entity.HasOne(x => x.Country)
                  .WithMany(x => x.States)
                  .HasForeignKey(x => x.CountryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(x => x.CityId);

            entity.Property(x => x.So).HasDefaultValue((byte)0);
            entity.Property(x => x.Active).HasDefaultValue((byte)1);

            entity.HasOne(x => x.State)
                  .WithMany(x => x.Cities)
                  .HasForeignKey(x => x.StateId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BusinessCategory>(entity =>
        {
            entity.HasKey(x => x.BusinessCategoryId);

            entity.Property(x => x.So).HasDefaultValue(0);
            entity.Property(x => x.Active).HasDefaultValue((byte)1);
        });

        modelBuilder.Entity<UserRefreshToken>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.Token).IsUnique();

            entity.HasOne(x => x.User)
                  .WithMany()
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BusinessInfo>(entity =>
        {
            entity.HasKey(x => x.BusinessInfoId);

            entity.HasIndex(x => x.UserId).IsUnique();

            entity.HasOne(x => x.User)
                  .WithMany()
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(x => x.GroupId);

            entity.Property(x => x.GroupCode)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.HasIndex(x => x.GroupCode)
                  .IsUnique();

            entity.Property(x => x.Active)
                  .HasDefaultValue((byte)2);

            entity.Property(x => x.BizonetApproved)
                  .HasDefaultValue(false);

            entity.HasOne(x => x.Owner)
                  .WithMany()
                  .HasForeignKey(x => x.OwnerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<GroupUser>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Active)
                  .HasDefaultValue((byte)2);

            entity.HasOne(x => x.Group)
                  .WithMany(x => x.Members)
                  .HasForeignKey(x => x.GroupId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.User)
                  .WithMany()
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.BusinessCategory)
                  .WithMany()
                  .HasForeignKey(x => x.BusinessCategoryId)
                  .OnDelete(DeleteBehavior.SetNull);

            // prevent duplicate join per category
            entity.HasIndex(x => new { x.UserId, x.GroupId, x.BusinessCategoryId })
                  .IsUnique();
        });
        modelBuilder.Entity<FollowupStatus>(entity =>
        {
            entity.HasKey(x => x.FollowupStatusId);
            entity.Property(x => x.So).HasDefaultValue((byte)0);
            entity.Property(x => x.Active).HasDefaultValue((byte)1);
        });

        modelBuilder.Entity<FollowupComment>(entity =>
        {
            entity.HasKey(x => x.FollowupCommentsId);
            entity.Property(x => x.So).HasDefaultValue((byte)0);
            entity.Property(x => x.Active).HasDefaultValue((byte)1);

            entity.HasOne(x => x.FollowupStatus)
                  .WithMany(x => x.Comments)
                  .HasForeignKey(x => x.FollowupStatusId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Referral>(entity =>
        {
            entity.HasKey(x => x.ReferralId);

            entity.HasOne(x => x.ReferredByUser)
                  .WithMany()
                  .HasForeignKey(x => x.ReferredByUserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ReferredToUser)
                  .WithMany()
                  .HasForeignKey(x => x.ReferredToUserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Group)
                  .WithMany()
                  .HasForeignKey(x => x.GroupId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ReferralOutsider>(entity =>
        {
            entity.HasKey(x => x.OutsiderId);

            entity.HasOne(x => x.Referral)
                  .WithOne(x => x.Outsider)
                  .HasForeignKey<ReferralOutsider>(x => x.ReferralId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ReferralStatus>(entity =>
        {
            entity.HasKey(x => x.ReferralStatusId);

            entity.HasOne(x => x.Referral)
                  .WithMany(x => x.StatusHistory)
                  .HasForeignKey(x => x.ReferralId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.FollowupStatus)
                  .WithMany()
                  .HasForeignKey(x => x.FollowupStatusId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.FollowupComment)
                  .WithMany()
                  .HasForeignKey(x => x.FollowupCommentsId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
