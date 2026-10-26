using Data.Telegram;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class TelegramSessionConfiguration : IEntityTypeConfiguration<TelegramSession>
{
    public void Configure(EntityTypeBuilder<TelegramSession> builder)
    {
        builder.ToTable("TelegramSessions");
        builder.HasKey(s => s.Key);
        builder.Property(s => s.Key).HasMaxLength(32);
        builder.Property(s => s.Data).IsRequired();
    }
}
