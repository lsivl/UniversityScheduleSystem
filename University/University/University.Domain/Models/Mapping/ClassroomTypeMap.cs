using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class ClassroomTypeMap : EntityTypeConfiguration<ClassroomType>
    {
        public ClassroomTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.ClassroomTypeID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(50);

            this.Property(t => t.ShortName)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("ClassroomType");
            this.Property(t => t.ClassroomTypeID).HasColumnName("ClassroomTypeID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ShortName).HasColumnName("ShortName");
        }
    }
}
