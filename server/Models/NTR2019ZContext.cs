using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NTR5.Models
{
    public partial class NTR2019ZContext : DbContext
    {
        public NTR2019ZContext()
        {
        }

        public NTR2019ZContext(DbContextOptions<NTR2019ZContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Note> Note { get; set; }
        public virtual DbSet<NoteCategory> NoteCategory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost,7423;Database=NTR2019Z;User Id=User2019Z;Password=Password2019Z;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Idcategory);

                entity.Property(e => e.Idcategory).HasColumnName("IDCategory");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(32);
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.HasKey(e => e.Idnote);

                entity.Property(e => e.Idnote).HasColumnName("IDNote");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(1024)
                    .IsFixedLength();

                entity.Property(e => e.Timestamp)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(32);
            });

            modelBuilder.Entity<NoteCategory>(entity =>
            {
                entity.HasKey(e => new { e.Idnote, e.Idcategory });

                entity.Property(e => e.Idcategory).HasColumnName("IDCategory");

                entity.Property(e => e.Idnote).HasColumnName("IDNote");

                entity.HasOne(d => d.IdcategoryNavigation)
                    .WithMany(p => p.NoteCategory)
                    .HasForeignKey(d => d.Idcategory)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NoteCategory_Category");

                entity.HasOne(d => d.IdnoteNavigation)
                    .WithMany(p => p.NoteCategory)
                    .HasForeignKey(d => d.Idnote)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NoteCategory_Note");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
