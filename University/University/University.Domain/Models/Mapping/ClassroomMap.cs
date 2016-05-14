using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class ClassroomMap : EntityTypeConfiguration<Classroom>
    {
        public ClassroomMap()
        {
            // Primary Key
            this.HasKey(t => t.ClassroomID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Classroom");
            this.Property(t => t.ClassroomID).HasColumnName("ClassroomID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Capacity).HasColumnName("Capacity");
            this.Property(t => t.ClassroomTypeID).HasColumnName("ClassroomTypeID");
            this.Property(t => t.CathedraID).HasColumnName("CathedraID");

            // Relationships
            this.HasOptional(t => t.Cathedra)
                .WithMany(t => t.Classrooms)
                .HasForeignKey(d => d.CathedraID);
            this.HasRequired(t => t.ClassroomType)
                .WithMany(t => t.Classrooms)
                .HasForeignKey(d => d.ClassroomTypeID);

        }
    }
}
