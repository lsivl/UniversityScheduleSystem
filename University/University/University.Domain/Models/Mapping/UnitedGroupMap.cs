using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace University.Domain.Models.Mapping
{
    public class UnitedGroupMap : EntityTypeConfiguration<UnitedGroup>
    {
        public UnitedGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.UnitedGroupsID);

            // Properties
            // Table & Column Mappings
            this.ToTable("UnitedGroups");
            this.Property(t => t.UnitedGroupsID).HasColumnName("UnitedGroupsID");
            this.Property(t => t.FirstGroupID).HasColumnName("FirstGroupID");
            this.Property(t => t.SecondGroupID).HasColumnName("SecondGroupID");
            this.Property(t => t.ThirdGroupID).HasColumnName("ThirdGroupID");
            this.Property(t => t.FourthGroupID).HasColumnName("FourthGroupID");
            this.Property(t => t.FifthGroupID).HasColumnName("FifthGroupID");

            // Relationships
            this.HasOptional(t => t.Group)
                .WithMany(t => t.UnitedGroups)
                .HasForeignKey(d => d.FifthGroupID);
            this.HasRequired(t => t.Group1)
                .WithMany(t => t.UnitedGroups1)
                .HasForeignKey(d => d.FirstGroupID);
            this.HasOptional(t => t.Group2)
                .WithMany(t => t.UnitedGroups2)
                .HasForeignKey(d => d.FourthGroupID);
            this.HasRequired(t => t.Group3)
                .WithMany(t => t.UnitedGroups3)
                .HasForeignKey(d => d.SecondGroupID);
            this.HasOptional(t => t.Group4)
                .WithMany(t => t.UnitedGroups4)
                .HasForeignKey(d => d.ThirdGroupID);

        }
    }
}
