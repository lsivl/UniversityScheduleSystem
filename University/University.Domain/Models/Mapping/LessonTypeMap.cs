using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class LessonTypeMap : EntityTypeConfiguration<LessonType>
    {
        public LessonTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.LessonTypeID);

            // Properties
            this.Property(t => t.LessonTypeName)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("LessonType");
            this.Property(t => t.LessonTypeID).HasColumnName("LessonTypeID");
            this.Property(t => t.LessonTypeName).HasColumnName("LessonTypeName");
        }
    }
}
