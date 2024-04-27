using Ecoeden.Notification.Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecoeden.Notification.Service.Data.Configurations
{
    public class NotificationHistoryConfiguration : IEntityTypeConfiguration<NotificationHistory>
    {
        public void Configure(EntityTypeBuilder<NotificationHistory> builder)
        {
            builder.ToTable("NotificationHistories");
            builder.HasIndex(x => x.Id).IsUnique();
        }
    }
}
