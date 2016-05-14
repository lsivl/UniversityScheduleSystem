using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class UnitedGroupMap : EntityTypeConfiguration<UnitedGroup>
    {
        public UnitedGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.UnitedGroupID);

            // Properties
            // Table & Column Mappings
            this.ToTable("UnitedGroups");
            this.Property(t => t.UnitedGroupID).HasColumnName("UnitedGroupID");
            this.Property(t => t.GroupID).HasColumnName("GroupID");
            this.Property(t => t.StreamID).HasColumnName("StreamID");
            this.Property(t => t.SubjectID).HasColumnName("SubjectID");
            this.Property(t => t.CathedraID).HasColumnName("CathedraID");
            this.Property(t => t.LessonTypeID).HasColumnName("LessonTypeID");
            this.Property(t => t.Course).HasColumnName("Course");
            this.Property(t => t.Row).HasColumnName("Row");

            // Relationships
            this.HasRequired(t => t.Cathedra)
                .WithMany(t => t.UnitedGroups)
                .HasForeignKey(d => d.CathedraID);
            this.HasRequired(t => t.Group)
                .WithMany(t => t.UnitedGroups)
                .HasForeignKey(d => d.GroupID);
            this.HasRequired(t => t.Stream)
                .WithMany(t => t.UnitedGroups)
                .HasForeignKey(d => d.StreamID);
            this.HasRequired(t => t.Subject)
                .WithMany(t => t.UnitedGroups)
                .HasForeignKey(d => d.SubjectID);

        }
    }
}
