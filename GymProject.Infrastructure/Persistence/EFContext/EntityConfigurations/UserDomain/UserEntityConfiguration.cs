﻿using GymProject.Domain.UserAccountDomain.UserAggregate;
using GymProject.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class UserEntityConfiguration : IEntityTypeConfiguration<UserRoot>
    {


        public void Configure(EntityTypeBuilder<UserRoot> builder)
        {
            builder.ToTable("User", GymContext.DefaultSchema);

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .ValueGeneratedOnAdd();

            builder.Property(u => u.Email)
                .IsRequired()
                .HasColumnType("TEXT");

            builder.HasAlternateKey(u => u.Email);

            builder.Property(u => u.UserName)
                .IsRequired()
                .HasColumnName("Username")
                .HasColumnType("TEXT");

            builder.HasAlternateKey(u => u.UserName);

            builder.Property(u => u.Password)
                .IsRequired()
                .HasColumnType("TEXT");


            builder.Property(u => u.Salt)
                .IsRequired()
                .HasColumnType("TEXT");

            builder.HasAlternateKey(u => u.Salt);

            builder.Property(u => u.SubscriptionDate)
                .IsRequired()
                .HasConversion(new UnixTimestampValueConverter())
                .HasColumnType("INTEGER")
                .HasDefaultValueSql(@"strftime('%s', 'now')");

            //// Data Seeding
            //builder.HasData(DataSeeding.GetUserNativeEntries());
        }

    }
}
