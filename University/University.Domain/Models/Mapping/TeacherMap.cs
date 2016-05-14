using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class TeacherMap : EntityTypeConfiguration<Teacher>
    {
        public TeacherMap()
        {
            // Primary Key
            this.HasKey(t => t.TeacherID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(50);

            this.Property(t => t.Post)
                .IsFixedLength()
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("Teacher");
            this.Property(t => t.TeacherID).HasColumnName("TeacherID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Post).HasColumnName("Post");
            this.Property(t => t.CathedraID).HasColumnName("CathedraID");

            // Relationships
            this.HasRequired(t => t.Cathedra)
                .WithMany(t => t.Teachers)
                .HasForeignKey(d => d.CathedraID);

        }
    }
}
