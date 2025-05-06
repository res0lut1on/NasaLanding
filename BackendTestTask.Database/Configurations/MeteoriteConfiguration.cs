using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackendTestTask.Database.Entities;

namespace BackendTestTask.Database.Configurations
{
    public class MeteoriteConfiguration : EntityConfiguration<Meteorite>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<Meteorite> builder)
        {
            builder.HasKey(m => m.Id);

            builder
                .HasIndex(m => m.Year);

            builder
                .HasIndex(m => m.Class);

            base.ConfigureEntity(builder);
        }
    }
}
