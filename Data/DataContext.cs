using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data.Extensions;
using Data.Models;
using Data.Models.Common;
using Data.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class DataContext : IdentityDbContext<User>
    {

        #region User
        public new DbSet<User> Users { get; set; }
        public new DbSet<Role> Roles { get; set; }
        #endregion

        #region Common
        public DbSet<Data.Models.Common.KeyValuePair> KeyValuePairs { get; set; }
        public DbSet<UserAgreement> UserAgreements { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<PrivacyPolicy> PrivacyPolicies { get; set; }
        #endregion

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region User
            builder.Entity<User>().ToTable("Users").HasIndex(t => t.VoucherId).IsUnique();
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityRole>().ToTable("Roles");
            #endregion

            #region Common
            builder.Entity<Data.Models.Common.KeyValuePair>().ToTable("KeyValuePairs").HasIndex(x => x.Key).IsUnique();
            builder.Entity<UserAgreement>().Property(t => t.Version).ValueGeneratedOnAdd();
            builder.Entity<UserAgreement>().Property(t => t.Version).Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);
            builder.Entity<UserAgreement>().ToTable("UserAgreements");

            builder.Entity<AboutUs>().Property(t => t.Version).ValueGeneratedOnAdd();
            builder.Entity<AboutUs>().Property(t => t.Version).Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);
            builder.Entity<AboutUs>().ToTable("AboutUs");

            builder.Entity<FAQ>().Property(t => t.Version).ValueGeneratedOnAdd();
            builder.Entity<FAQ>().Property(t => t.Version).Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);
            builder.Entity<FAQ>().ToTable("FAQs");

            builder.Entity<PrivacyPolicy>().Property(t => t.Version).ValueGeneratedOnAdd();
            builder.Entity<PrivacyPolicy>().Property(t => t.Version).Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);
            builder.Entity<PrivacyPolicy>().ToTable("PrivacyPolicies");
            #endregion

            #region Filter
            builder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
            #endregion

            builder.Seed();
        }


        private void OrganizeDates()
        {
            var now = DateTimeOffset.UtcNow;

            var addedAuditedEntities = ChangeTracker.Entries<IBaseEntity>().Where(p => p.State == EntityState.Added).Select(p => p.Entity);

            foreach (var added in addedAuditedEntities)
            {
                //added.CreateDate = now;
            }

            var modifiedAuditedEntities = ChangeTracker.Entries<IAuditedEntity>()
                .Where(p => p.State == EntityState.Modified)
                .Select(p => p.Entity);

            foreach (var modified in modifiedAuditedEntities)
            {
                modified.LastModifiedDate = now;
            }

            var removedEntities = ChangeTracker.Entries<IBaseEntity>().Where(p => p.State == EntityState.Deleted)
                .Select(p => p.Entity);
            foreach (var deleted in removedEntities)
            {
                deleted.DeleteDate = now;
                deleted.IsDeleted = true;
            }

        }

        public Task<int> SaveChangesAsync()
        {
            OrganizeDates();
            return base.SaveChangesAsync();
        }

        public override int SaveChanges()
        {
            OrganizeDates();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            OrganizeDates();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            OrganizeDates();
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
