using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NattiChatBot.Domain;

namespace NattiChatBot.Data.ModelConfigurations;

public class StatsConfiguration : IEntityTypeConfiguration<Stats>
{
    public void Configure(EntityTypeBuilder<Stats> builder)
    {
        builder.HasIndex(x => x.Date).IsUnique();
        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.NewMembersCount).IsRequired();
        builder.Property(x => x.MessagesCount).IsRequired();
    }
}