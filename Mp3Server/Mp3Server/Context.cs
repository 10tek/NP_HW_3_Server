using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mp3Server
{
    public class Context : DbContext
    {
        public Context()
        {
            Database.EnsureCreated();
        }

        public DbSet<MusicFile> MusicFiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-TALUG4B\SQLEXPRESS;Database=MusicAppDb;Trusted_Connection=True");
        }
    }
}
