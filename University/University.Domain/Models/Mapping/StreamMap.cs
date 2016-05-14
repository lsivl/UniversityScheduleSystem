using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class StreamMap : EntityTypeConfiguration<Stream>
    {
        public StreamMap()
        {
            // Primary Key
            this.HasKey(t => t.StreamID);

            // Properties
            this.Property(t => t.StreamName)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("Stream");
            this.Property(t => t.StreamID).HasColumnName("StreamID");
            this.Property(t => t.StreamName).HasColumnName("StreamName");
            this.Property(t => t.YearOfStudy).HasColumnName("YearOfStudy");
            this.Property(t => t.FacultyID).HasColumnName("FacultyID");
            this.Property(t => t.StudentsCount).HasColumnName("StudentsCount");

            // Relationships
            this.HasOptional(t => t.Faculty)
                .WithMany(t => t.Streams)
                .HasForeignKey(d => d.FacultyID);

        }
    }
}
