using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class StreamSubjectBridgeMap : EntityTypeConfiguration<StreamSubjectBridge>
    {
        public StreamSubjectBridgeMap()
        {
            // Primary Key
            this.HasKey(t => t.StreamSubjectBridgeID);

            // Properties
            // Table & Column Mappings
            this.ToTable("StreamSubjectBridge");
            this.Property(t => t.StreamSubjectBridgeID).HasColumnName("StreamSubjectBridgeID");
            this.Property(t => t.StreamID).HasColumnName("StreamID");
            this.Property(t => t.SubjectID).HasColumnName("SubjectID");
            this.Property(t => t.GroupID).HasColumnName("GroupID");
            this.Property(t => t.LessonTypeID).HasColumnName("LessonTypeID");
            this.Property(t => t.TeacherID).HasColumnName("TeacherID");
            this.Property(t => t.ClassroomID).HasColumnName("ClassroomID");
            this.Property(t => t.CountHours).HasColumnName("CountHours");
            this.Property(t => t.Сoupled).HasColumnName("Сoupled");
            this.Property(t => t.CathedraID).HasColumnName("CathedraID");

            // Relationships
            this.HasOptional(t => t.Cathedra)
                .WithMany(t => t.StreamSubjectBridges)
                .HasForeignKey(d => d.CathedraID);
            this.HasOptional(t => t.Classroom)
                .WithMany(t => t.StreamSubjectBridges)
                .HasForeignKey(d => d.ClassroomID);
            this.HasRequired(t => t.Group)
                .WithMany(t => t.StreamSubjectBridges)
                .HasForeignKey(d => d.GroupID);
            this.HasRequired(t => t.LessonType)
                .WithMany(t => t.StreamSubjectBridges)
                .HasForeignKey(d => d.LessonTypeID);
            this.HasRequired(t => t.Stream)
                .WithMany(t => t.StreamSubjectBridges)
                .HasForeignKey(d => d.StreamID);
            this.HasRequired(t => t.Teacher)
                .WithMany(t => t.StreamSubjectBridges)
                .HasForeignKey(d => d.TeacherID);
            this.HasRequired(t => t.Subject)
                .WithMany(t => t.StreamSubjectBridges)
                .HasForeignKey(d => d.SubjectID);

        }
    }
}
