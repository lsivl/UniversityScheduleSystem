using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class TeacherSubjectBridgeMap : EntityTypeConfiguration<TeacherSubjectBridge>
    {
        public TeacherSubjectBridgeMap()
        {
            // Primary Key
            this.HasKey(t => t.TeacherSubjectBridgeID);

            // Properties
            this.Property(t => t.TeacherSubjectBridgeID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("TeacherSubjectBridge");
            this.Property(t => t.TeacherSubjectBridgeID).HasColumnName("TeacherSubjectBridgeID");
            this.Property(t => t.TeacherID).HasColumnName("TeacherID");
            this.Property(t => t.SubjectID).HasColumnName("SubjectID");
            this.Property(t => t.LessonTypeID).HasColumnName("LessonTypeID");

            // Relationships
            this.HasRequired(t => t.LessonType)
                .WithMany(t => t.TeacherSubjectBridges)
                .HasForeignKey(d => d.LessonTypeID);
            this.HasRequired(t => t.Subject)
                .WithMany(t => t.TeacherSubjectBridges)
                .HasForeignKey(d => d.SubjectID);
            this.HasRequired(t => t.Teacher)
                .WithMany(t => t.TeacherSubjectBridges)
                .HasForeignKey(d => d.TeacherID);

        }
    }
}
