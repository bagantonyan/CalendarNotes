﻿using CalendarNotes.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CalendarNotes.DAL.Configurations
{
    internal abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasQueryFilter(f => EF.Property<DateTime?>(f, "DeletedDate") == null);

            builder.Property(p => p.CreatedDate)
                .IsRequired(true);

            builder.Property(p => p.ModifiedDate)
                .IsRequired(true);

            builder.Property(p => p.DeletedDate)
                .IsRequired(false);
        }
    }
}