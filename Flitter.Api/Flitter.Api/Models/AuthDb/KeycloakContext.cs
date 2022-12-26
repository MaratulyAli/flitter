using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Flitter.Api.Models.AuthDb
{
    public partial class KeycloakContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public KeycloakContext(DbContextOptions<KeycloakContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Follow> Follows { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(_configuration.GetConnectionString("AuthDb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user_entity");

                entity.HasIndex(e => e.Email, "idx_user_email");

                entity.HasIndex(e => new { e.RealmId, e.EmailConstraint }, "uk_dykn684sl8up1crfei6eckhd7")
                    .IsUnique();

                entity.HasIndex(e => new { e.RealmId, e.UserName }, "uk_ru8tt6t700s9v50bu18ws5ha6")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .HasColumnName("id");

                entity.Property(e => e.CreatedTimestamp).HasColumnName("created_timestamp");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.EmailConstraint)
                    .HasMaxLength(255)
                    .HasColumnName("email_constraint");

                entity.Property(e => e.EmailVerified).HasColumnName("email_verified");

                entity.Property(e => e.Enabled).HasColumnName("enabled");

                entity.Property(e => e.FederationLink)
                    .HasMaxLength(255)
                    .HasColumnName("federation_link");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(255)
                    .HasColumnName("first_name");

                entity.Property(e => e.LastName)
                    .HasMaxLength(255)
                    .HasColumnName("last_name");

                entity.Property(e => e.NotBefore).HasColumnName("not_before");

                entity.Property(e => e.RealmId)
                    .HasMaxLength(255)
                    .HasColumnName("realm_id");

                entity.Property(e => e.ServiceAccountClientLink)
                    .HasMaxLength(255)
                    .HasColumnName("service_account_client_link");

                entity.Property(e => e.UserName)
                    .HasMaxLength(255)
                    .HasColumnName("username");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
