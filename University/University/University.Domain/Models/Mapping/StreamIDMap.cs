using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class StreamIDMap : EntityTypeConfiguration<StreamID>
    {
        public StreamIDMap()
        {
            // Primary Key
            this.HasKey(t => t.StreamID1);

            // Properties
            this.Property(t => t.StreamID1)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("StreamID");
            this.Property(t => t.StreamID1).HasColumnName("StreamID");
        }
    }
}
