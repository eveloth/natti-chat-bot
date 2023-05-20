using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NattiChatBot.Domain;

namespace NattiChatBot.Data.ModelConfigurations;

public class TokenConfiguration : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.HasIndex(x => x.AccessToken).IsUnique();

        builder.Property(x => x.IssuedAt).IsRequired();
        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.Property(x => x.GrantedTo).IsRequired().HasMaxLength(64);
        builder.Property(x => x.AccessToken).IsRequired().HasMaxLength(64);
        builder.Property(x => x.AccessType).IsRequired();
    }
}