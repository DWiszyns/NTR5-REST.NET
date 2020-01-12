
// using System;
// using System.Collections.Generic;
// using System.Text;
// using Microsoft.EntityFrameworkCore;
// using NTR5.OldModels;

// namespace NTR5.Data
// {
//     public class ApplicationDbContext : DbContext
//     {
//         public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
//             : base(options)
//         {
//         }
//         protected override void OnModelCreating(ModelBuilder builder)
//         {
//             base.OnModelCreating(builder);
//             builder.Entity<Category>()
//             .HasIndex(b => b.Title).IsUnique();
//             builder.Entity<Note>()
//             .HasIndex(b => b.Title).IsUnique();
//             builder.Entity<NoteCategory>()
//             .HasKey(c => new { c.CategoryID, c.NoteID });
//         }
//         public DbSet<NTR5.OldModels.Note> Notes { get; set; }
//         public DbSet<NTR5.OldModels.Category> Categories { get; set; }
//         public DbSet<NTR5.OldModels.NoteCategory> NoteCategories { get; set; }

//     }
// }