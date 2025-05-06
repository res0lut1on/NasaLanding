using BackendTestTask.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BackendTestTask.Database
{
    public class BackendTestTaskContext : BaseContext
    {
        public DbSet<Meteorite> Meteorites { get; set; } = null!;

        public BackendTestTaskContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BackendTestTaskContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
