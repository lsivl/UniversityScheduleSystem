using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class vIDLesoonEventMap : EntityTypeConfiguration<vIDLesoonEvent>
    {
        public vIDLesoonEventMap()
        {
            // Primary Key
            this.HasKey(t => t.LessonEventID);

            // Properties
            // Table & Column Mappings
            this.ToTable("vIDLesoonEvent");
            this.Property(t => t.LessonEventID).HasColumnName("LessonEventID");
        }
    }
}
