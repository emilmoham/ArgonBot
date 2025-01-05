using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArgonBot.Models.Entities
{
    public class User
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public uint ChannelPoints { get; set; }
        
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .ToTable("Users")
                .HasKey(e => e.UserId);

            builder
                .Property(e => e.UserName)
                .HasMaxLength(50);

            builder
                .Property(e => e.ChannelPoints)
                .HasDefaultValue(0);
        }
    }
}
