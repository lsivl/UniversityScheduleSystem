using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class vStreamIDMap : EntityTypeConfiguration<vStreamID>
    {
        public vStreamIDMap()
        {
            // Primary Key
            this.HasKey(t => t.StreamID);

            // Properties
            this.Property(t => t.StreamID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("vStreamID");
            this.Property(t => t.StreamID).HasColumnName("StreamID");
        }
    }
}
