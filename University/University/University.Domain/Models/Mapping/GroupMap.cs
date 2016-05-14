using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class GroupMap : EntityTypeConfiguration<Group>
    {
        public GroupMap()
        {
            // Primary Key
            this.HasKey(t => t.GroupID);

            // Properties
            this.Property(t => t.GroupName)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("Group");
            this.Property(t => t.GroupID).HasColumnName("GroupID");
            this.Property(t => t.GroupName).HasColumnName("GroupName");
            this.Property(t => t.StudentCount).HasColumnName("StudentCount");
            this.Property(t => t.StreamID).HasColumnName("StreamID");

            // Relationships
            this.HasRequired(t => t.Stream)
                .WithMany(t => t.Groups)
                .HasForeignKey(d => d.StreamID);

        }
    }
}
