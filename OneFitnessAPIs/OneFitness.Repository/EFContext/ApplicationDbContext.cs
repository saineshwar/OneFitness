using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;

namespace OneFitness.Repository.EFContext
{
    public abstract class ApplicationDbContext : DbContext
    {
        protected ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<WorkOut> WorkOuts { get; set; }

        public DbSet<MembershipType> MembershipTypes { get; set; }

        public DbSet<PaymentType> PaymentTypes { get; set; }

        public DbSet<Installment> Installments { get; set; }

        public DbSet<TaxMaster> TaxMasters { get; set; }

        public DbSet<Reason> Reasons { get; set; }

        public DbSet<Enquiry> Enquiries { get; set; }

        public DbSet<GeneralSettings> GeneralSettings { get; set; }

        public DbSet<MenuCategory> MenuCategories { get; set; }

        public DbSet<MenuMaster> MenuMasters { get; set; }

        public DbSet<SubMenuMaster> SubMenuMasters { get; set; }

        public DbSet<Member> Members { get; set; }

        public DbSet<MemberPhoto> MemberPhotos { get; set; }

        public DbSet<RoleMaster> RoleMasters { get; set; }

        public DbSet<Refund> Refunds { get; set; }

        public DbSet<ReceiptHistory> ReceiptHistories { get; set; }

        public DbSet<AssignedRole> AssignedRoles { get; set; }

        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.UserName).IsUnique();
                entity.HasIndex(u => u.EmailId).IsUnique();
            });

            modelBuilder.Entity<WorkOut>(entity =>
            {
                entity.HasIndex(w => w.WorkOutName).IsUnique();
            });

            modelBuilder.Entity<MembershipType>(entity =>
            {
                entity.HasIndex(m => m.MembershipTypeName).IsUnique();
            });

            modelBuilder.Entity<PaymentType>(entity =>
            {
                entity.HasIndex(p => p.PaymentTypeName).IsUnique();
            });

            modelBuilder.Entity<Installment>(entity =>
            {
                entity.HasIndex(i => i.InstallmentName).IsUnique();
            });

            modelBuilder.Entity<TaxMaster>(entity =>
            {
                entity.HasIndex(t => t.TaxType).IsUnique();
            });

            modelBuilder.Entity<Reason>(entity =>
            {
                entity.HasIndex(r => r.ReasonName).IsUnique();
            });

            modelBuilder.Entity<Enquiry>(entity =>
            {
                entity.HasIndex(e => e.MobileNo).IsUnique().HasFilter("[MobileNo] IS NOT NULL");
                entity.HasIndex(e => e.EmailId).IsUnique().HasFilter("[EmailId] IS NOT NULL");
            });

            modelBuilder.Entity<MenuCategory>(entity =>
            {
                entity.HasIndex(m => new { m.MenuCategoryName, m.RoleId }).IsUnique();
            });

            modelBuilder.Entity<MenuMaster>(entity =>
            {
                entity.HasIndex(m => new { m.MenuName, m.RoleId, m.MenuCategoryId }).IsUnique();
            });

            modelBuilder.Entity<SubMenuMaster>(entity =>
            {
                entity.HasIndex(s => new { s.SubMenuName, s.MenuId, s.RoleId, s.MenuCategoryId }).IsUnique();
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasIndex(m => m.MemberNo).IsUnique();
                entity.HasIndex(m => m.MobileNo).IsUnique().HasFilter("[MobileNo] IS NOT NULL");
                entity.HasIndex(m => m.EmailId).IsUnique().HasFilter("[EmailId] IS NOT NULL");
            });

            modelBuilder.Entity<MemberPhoto>(entity =>
            {
                entity.HasOne<Member>()
                    .WithOne()
                    .HasForeignKey<MemberPhoto>(p => p.MemberId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RoleMaster>(entity =>
            {
                entity.HasIndex(r => r.RoleName).IsUnique();
            });

            modelBuilder.Entity<AssignedRole>(entity =>
            {
                entity.HasIndex(a => a.UserId).IsUnique();
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasIndex(p => p.MemberId);
                entity.HasOne<Member>()
                    .WithMany()
                    .HasForeignKey(p => p.MemberId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
