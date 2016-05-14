using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class CathedraMap : EntityTypeConfiguration<Cathedra>
    {
        public CathedraMap()
        {
            // Primary Key
            this.HasKey(t => t.CathedraID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(10);

            this.Property(t => t.FullName)
                .IsFixedLength()
                .HasMaxLength(70);

            // Table & Column Mappings
            this.ToTable("Cathedra");
            this.Property(t => t.CathedraID).HasColumnName("CathedraID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.FacultyID).HasColumnName("FacultyID");
            this.Property(t => t.FullName).HasColumnName("FullName");

            // Relationships
            this.HasOptional(t => t.Faculty)
                .WithMany(t => t.Cathedras)
                .HasForeignKey(d => d.FacultyID);

        }
    }
}
