using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace conversion_api.Models
{
    public partial class DBContext : DbContext
    {
        public DBContext()
        {
        }

        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Campaign> Campaigns { get; set; }
        public virtual DbSet<CampaignStore> CampaignStores { get; set; }
        public virtual DbSet<CampaignStoreModule> CampaignStoreModules { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<ModuleContact> ModuleContacts { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<ContentBlock> ContentBlocks { get; set; }
        public virtual DbSet<CampaignStatistic> CampaignStatistics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");

            modelBuilder.Entity<Campaign>(entity =>
            {
                entity.ToTable("campaign");

                entity.HasIndex(e => e.CompanyId, "company_campaign_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CompanyId).HasColumnName("company_id");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime")
                    .HasColumnName("end_date");

                entity.Property(e => e.HeaderColor)
                    .HasMaxLength(20)
                    .HasColumnName("header_color");

                entity.Property(e => e.HeaderText)
                    .HasMaxLength(500)
                    .HasColumnName("header_text");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("modified_date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("start_date");

                entity.Property(e => e.TextColor)
                    .HasMaxLength(20)
                    .HasColumnName("text_color");

                entity.Property(e => e.UniqueUrl)
                    .IsRequired()
                    .HasMaxLength(2000)
                    .HasColumnName("unique_url");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Campaigns)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("company_campaign");
            });

            modelBuilder.Entity<CampaignStore>(entity =>
            {
                entity.ToTable("campaign_store");

                entity.HasIndex(e => e.StoreId, "campaign_store_fk_idx");

                entity.HasIndex(e => e.CampaignId, "company_store_fk_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CampaignId).HasColumnName("campaign_id");

                entity.Property(e => e.StoreId).HasColumnName("store_id");

                entity.Property(e => e.UniqueUrl)
                    .IsRequired()
                    .HasMaxLength(2000)
                    .HasColumnName("unique_url");

                entity.Property(e => e.ShortCode)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("short_code");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

                entity.HasOne(d => d.Campaign)
                    .WithMany(p => p.CampaignStores)
                    .HasForeignKey(d => d.CampaignId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("campaign_campaign_fk");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.CampaignStores)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("campaign_store_fk");
            });

            modelBuilder.Entity<CampaignStoreModule>(entity =>
            {
                entity.ToTable("campaign_store_module");

                entity.HasIndex(e => e.CampaignId, "campaign_store_module_fk_idx");

                entity.HasIndex(e => e.ModuleId, "campaign_store_module_module_fk");

                entity.HasIndex(e => e.StoreId, "campaign_store_module_store_fk_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CampaignId).HasColumnName("campaign_id");

                entity.Property(e => e.CampaignStoreId).HasColumnName("campaign_store_id");

                entity.Property(e => e.ModuleId).HasColumnName("module_id");

                entity.Property(e => e.Content)
                    .HasColumnName("content").HasColumnType("longText");

                entity.Property(e => e.StoreId).HasColumnName("store_id");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

                entity.HasOne(d => d.Campaign)
                    .WithMany(p => p.CampaignStoreModules)
                    .HasForeignKey(d => d.CampaignId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("campaign_store_module_fk");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.CampaignStoreModules)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("campaign_store_module_module_fk");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.CampaignStoreModules)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("campaign_store_module_store_fk");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("company");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

                entity.Property(e => e.CalendarUrl)
                    .HasMaxLength(2000)
                    .HasColumnName("calendar_url");

                entity.Property(e => e.ContactFormUrl)
                    .HasMaxLength(2000)
                    .HasColumnName("contact_form_url");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.FacebookUrl)
                    .HasMaxLength(2000)
                    .HasColumnName("facebook_url");

                entity.Property(e => e.FooterText)
                    .HasColumnType("text")
                    .HasColumnName("footer_text");

                entity.Property(e => e.HeaderText)
                    .HasColumnType("text")
                    .HasColumnName("header_text");

                entity.Property(e => e.LogoUrl)
                    .HasMaxLength(2000)
                    .HasColumnName("logo_url");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("modified_date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.WebsiteUrl)
                    .HasMaxLength(2000)
                    .HasColumnName("website_url");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.ToTable("module");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Type)
                    .HasMaxLength(100)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<ModuleContact>(entity =>
            {
                entity.ToTable("module_contact");

                entity.HasIndex(e => e.CampaignId, "module_contact_campaign_fk_idx");

                entity.HasIndex(e => e.StoreId, "module_contact_store_fk_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CampaignId).HasColumnName("campaign_id");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.Email)
                    .HasMaxLength(80)
                    .HasColumnName("email");

                entity.Property(e => e.Firstname)
                    .IsRequired()
                    .HasMaxLength(80)
                    .HasColumnName("firstname");

                entity.Property(e => e.Lastname)
                    .IsRequired()
                    .HasMaxLength(80)
                    .HasColumnName("lastname");

                entity.Property(e => e.ModuleId).HasColumnName("module_id");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .HasColumnName("phone");

                entity.Property(e => e.StoreId).HasColumnName("store_id");

                entity.HasOne(d => d.Campaign)
                    .WithMany(p => p.ModuleContacts)
                    .HasForeignKey(d => d.CampaignId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("module_contact_campaign_fk");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.ModuleContacts)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("module_contact_store_fk");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.ToTable("store");

                entity.HasIndex(e => e.CompanyId, "company_store_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address1)
                    .HasMaxLength(500)
                    .HasColumnName("address1");

                entity.Property(e => e.CalendarUrl)
                    .HasMaxLength(2000)
                    .HasColumnName("calendar_url");

                entity.Property(e => e.City)
                    .HasMaxLength(80)
                    .HasColumnName("city");

                entity.Property(e => e.CompanyId).HasColumnName("company_id");

                entity.Property(e => e.ContactFormUrl)
                    .HasMaxLength(2000)
                    .HasColumnName("contact_form_url");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.Email)
                    .HasMaxLength(80)
                    .HasColumnName("email");

                entity.Property(e => e.FacebookUrl)
                    .HasMaxLength(2000)
                    .HasColumnName("facebook_url");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

                entity.Property(e => e.LogoUrl)
                    .HasMaxLength(2000)
                    .HasColumnName("logo_url");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("modified_date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Phone)
                    .HasMaxLength(15)
                    .HasColumnName("phone");

                entity.Property(e => e.State)
                    .HasMaxLength(80)
                    .HasColumnName("state");

                entity.Property(e => e.StoreId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("store_id");

                entity.Property(e => e.UniqueUrl)
                    .IsRequired()
                    .HasMaxLength(2000)
                    .HasColumnName("unique_url");

                entity.Property(e => e.WebsiteUrl)
                    .HasMaxLength(2000)
                    .HasColumnName("website_url");

                entity.Property(e => e.Zipcode)
                    .HasMaxLength(16)
                    .HasColumnName("zipcode");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("company_store");
            });

            modelBuilder.Entity<ContentBlock>(entity =>
            {
                entity.ToTable("content_block");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

                entity.Property(e => e.Content)
                    .HasColumnName("content").HasColumnType("longText");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("modified_date");
            });

            modelBuilder.Entity<CampaignStatistic>(entity =>
            {
                entity.ToTable("campaign_statistic");

                entity.HasIndex(e => e.CampaignId, "cms_campaign_idx");

                entity.HasIndex(e => e.StoreId, "cms_store_idx");

                entity.HasIndex(e => e.ModuleId, "cms_module_idx");

                entity.HasIndex(e => e.CampaignStoreId, "cms_campaign_store");

                entity.HasIndex(e => e.CampaignStoreModuleId, "cms_campaign_store_module_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CampaignId).HasColumnName("campaign_id");

                entity.Property(e => e.StoreId).HasColumnName("store_id");

                entity.Property(e => e.ModuleId).HasColumnName("module_id");

                entity.Property(e => e.CampaignStoreId).HasColumnName("campaign_store_id");

                entity.Property(e => e.CampaignStoreModuleId).HasColumnName("campaign_store_module_id");

                entity.Property(e => e.Browser)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("browser");

                entity.Property(e => e.IPAddress)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("ip_address");

                entity.Property(e => e.OS)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("os");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
