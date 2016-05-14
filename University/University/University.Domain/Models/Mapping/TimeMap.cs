using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class TimeMap : EntityTypeConfiguration<Time>
    {
        public TimeMap()
        {
            // Primary Key
            this.HasKey(t => t.TimeID);

            // Properties
            this.Property(t => t.DayOfWeek)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("Time");
            this.Property(t => t.TimeID).HasColumnName("TimeID");
            this.Property(t => t.WeekNum).HasColumnName("WeekNum");
            this.Property(t => t.DayOfWeek).HasColumnName("DayOfWeek");
            this.Property(t => t.LessonTimeID).HasColumnName("LessonTimeID");
            this.Property(t => t.DayNumber).HasColumnName("DayNumber");

            // Relationships
            this.HasRequired(t => t.LessonTime)
                .WithMany(t => t.Times)
                .HasForeignKey(d => d.LessonTimeID);

        }
    }
}
