using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class FacultyMap : EntityTypeConfiguration<Faculty>
    {
        public FacultyMap()
        {
            // Primary Key
            this.HasKey(t => t.FacultyID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(10);

            this.Property(t => t.FullName)
                .IsFixedLength()
                .HasMaxLength(70);

            // Table & Column Mappings
            this.ToTable("Faculty");
            this.Property(t => t.FacultyID).HasColumnName("FacultyID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.FullName).HasColumnName("FullName");
        }
    }
}
