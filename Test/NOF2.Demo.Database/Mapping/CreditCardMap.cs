using System.Data.Entity.ModelConfiguration;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<CreditCard> builder)
        {
            builder.HasKey(t => t.CreditCardID);

            //Ignores
            //builder.Ignore(t => t.Creator);
            //builder.Ignore(t => t.ForContact);
            //builder.Ignore(t => t.ObfuscatedNumber);

            // Properties
            builder.Property(t => t.CardType)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(t => t.CardNumber)
                   .IsRequired()
                   .HasMaxLength(25);

            // Table & Column Mappings
            builder.ToTable("CreditCard", "Sales");
            builder.Property(t => t.CreditCardID).HasColumnName("CreditCardID");
            builder.Property(t => t.CardType).HasColumnName("CardType");
            builder.Property(t => t.CardNumber).HasColumnName("CardNumber");
            builder.Property(t => t.ExpMonth).HasColumnName("ExpMonth");
            builder.Property(t => t.ExpYear).HasColumnName("ExpYear");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            builder.HasMany(t => t.mappedPersonLinks).WithOne(t => t.CreditCard);
        }
    }
}
