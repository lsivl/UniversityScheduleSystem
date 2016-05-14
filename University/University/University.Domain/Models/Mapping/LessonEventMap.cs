using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class LessonEventMap : EntityTypeConfiguration<LessonEvent>
    {
        public LessonEventMap()
        {
            // Primary Key
            this.HasKey(t => t.LessonEventID);

            // Properties
            // Table & Column Mappings
            this.ToTable("LessonEvent");
            this.Property(t => t.LessonEventID).HasColumnName("LessonEventID");
            this.Property(t => t.TimeID).HasColumnName("TimeID");
            this.Property(t => t.SubjectID).HasColumnName("SubjectID");
            this.Property(t => t.LessonTypeID).HasColumnName("LessonTypeID");
            this.Property(t => t.TeacherID).HasColumnName("TeacherID");
            this.Property(t => t.GroupID).HasColumnName("GroupID");
            this.Property(t => t.StreamID).HasColumnName("StreamID");
            this.Property(t => t.ClassroomID).HasColumnName("ClassroomID");

            // Relationships
            this.HasRequired(t => t.Classroom)
                .WithMany(t => t.LessonEvents)
                .HasForeignKey(d => d.ClassroomID);
            this.HasRequired(t => t.Group)
                .WithMany(t => t.LessonEvents)
                .HasForeignKey(d => d.GroupID);
            this.HasRequired(t => t.LessonType)
                .WithMany(t => t.LessonEvents)
                .HasForeignKey(d => d.LessonTypeID);
            this.HasRequired(t => t.Stream)
                .WithMany(t => t.LessonEvents)
                .HasForeignKey(d => d.StreamID);
            this.HasRequired(t => t.Subject)
                .WithMany(t => t.LessonEvents)
                .HasForeignKey(d => d.SubjectID);
            this.HasRequired(t => t.Teacher)
                .WithMany(t => t.LessonEvents)
                .HasForeignKey(d => d.TeacherID);
            this.HasRequired(t => t.Time)
                .WithMany(t => t.LessonEvents)
                .HasForeignKey(d => d.TimeID);

        }
    }
}
