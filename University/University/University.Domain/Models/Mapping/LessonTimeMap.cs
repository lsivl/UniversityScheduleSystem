using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class LessonTimeMap : EntityTypeConfiguration<LessonTime>
    {
        public LessonTimeMap()
        {
            // Primary Key
            this.HasKey(t => t.LessonTimeID);

            // Properties
            // Table & Column Mappings
            this.ToTable("LessonTime");
            this.Property(t => t.LessonTimeID).HasColumnName("LessonTimeID");
            this.Property(t => t.StartTime).HasColumnName("StartTime");
            this.Property(t => t.EndTime).HasColumnName("EndTime");
        }
    }
}
