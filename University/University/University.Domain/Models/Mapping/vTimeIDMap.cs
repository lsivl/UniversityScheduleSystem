using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class vTimeIDMap : EntityTypeConfiguration<vTimeID>
    {
        public vTimeIDMap()
        {
            // Primary Key
            this.HasKey(t => t.TimeID);

            // Properties
            // Table & Column Mappings
            this.ToTable("vTimeID");
            this.Property(t => t.TimeID).HasColumnName("TimeID");
        }
    }
}
