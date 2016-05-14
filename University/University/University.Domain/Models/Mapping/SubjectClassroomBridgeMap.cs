using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class SubjectClassroomBridgeMap : EntityTypeConfiguration<SubjectClassroomBridge>
    {
        public SubjectClassroomBridgeMap()
        {
            // Primary Key
            this.HasKey(t => t.SubjecyClassroomBridgeID);

            // Properties
            this.Property(t => t.SubjecyClassroomBridgeID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("SubjectClassroomBridge");
            this.Property(t => t.SubjecyClassroomBridgeID).HasColumnName("SubjecyClassroomBridgeID");
            this.Property(t => t.SubjectID).HasColumnName("SubjectID");
            this.Property(t => t.ClassroomID).HasColumnName("ClassroomID");
            this.Property(t => t.LessonTypeID).HasColumnName("LessonTypeID");

            // Relationships
            this.HasRequired(t => t.Classroom)
                .WithMany(t => t.SubjectClassroomBridges)
                .HasForeignKey(d => d.ClassroomID);
            this.HasRequired(t => t.LessonType)
                .WithMany(t => t.SubjectClassroomBridges)
                .HasForeignKey(d => d.LessonTypeID);
            this.HasRequired(t => t.Subject)
                .WithMany(t => t.SubjectClassroomBridges)
                .HasForeignKey(d => d.SubjectID);

        }
    }
}
