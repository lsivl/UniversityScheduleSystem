using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class FileTableMap : EntityTypeConfiguration<FileTable>
    {
        public FileTableMap()
        {
            // Primary Key
            this.HasKey(t => t.FileID);

            // Properties
            this.Property(t => t.FileName)
                .IsFixedLength()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("FileTable");
            this.Property(t => t.FileID).HasColumnName("FileID");
            this.Property(t => t.FileName).HasColumnName("FileName");
            this.Property(t => t.FileContent).HasColumnName("FileContent");
        }
    }
}
