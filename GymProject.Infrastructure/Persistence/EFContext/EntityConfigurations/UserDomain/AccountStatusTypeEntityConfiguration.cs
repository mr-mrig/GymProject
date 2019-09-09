using GymProject.Domain.UserAccountDomain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymProject.Infrastructure.Persistence.EFContext.EntityConfigurations.TrainingDomain
{
    internal class AccountStatusTypeEntityConfiguration : IEntityTypeConfiguration<AccountStatusTypeEnum>
    {


        public void Configure(EntityTypeBuilder<AccountStatusTypeEnum> builder)
        {
            builder.ToTable("AccountStatusType", GymContext.DefaultSchema);

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .ValueGeneratedNever();


            builder.Property(a => a.Name)
                .IsRequired()
                .HasColumnType("TEXT");

            builder.HasAlternateKey(a => a.Name);

            builder.HasMany<UserRoot>()
                .WithOne(u => u.AccountStatusType);

            // Data Seeding
            builder.HasData(AccountStatusTypeEnum.List());
        }

    }
}
