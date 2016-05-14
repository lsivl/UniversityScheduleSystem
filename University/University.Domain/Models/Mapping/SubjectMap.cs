using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class SubjectMap : EntityTypeConfiguration<Subject>
    {
        public SubjectMap()
        {
            // Primary Key
            this.HasKey(t => t.SubjectID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Subject");
            this.Property(t => t.SubjectID).HasColumnName("SubjectID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.CathedraID).HasColumnName("CathedraID");

            // Relationships
            this.HasRequired(t => t.Cathedra)
                .WithMany(t => t.Subjects)
                .HasForeignKey(d => d.CathedraID);

        }
    }
}
