using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Web;

namespace FunWithSignalR.Models
{
    public class ZigChatContext : DbContext
    {
        public ZigChatContext() : base("FunWithSignalR")
        {

        }

        public DbSet<Connection> Connections { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Connection>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Connection>()
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Connection>()
                .Property(x => x.UserName)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute { IsUnique = true }))
                .HasMaxLength(20);

            base.OnModelCreating(modelBuilder);
        }
    }
}