using REITs.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REITs.DataLayer.Configurations
{
    public class SystemAdminConfiguration : EntityTypeConfiguration<SystemAdmin>
    {
        public SystemAdminConfiguration()
        {
            HasKey(a => a.Id);
            Property(a => a.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(a => a.AdminType)
                    .HasMaxLength(30)
                    .IsRequired();

            Property(a => a.Description)
                    .HasMaxLength(50)
                    .IsRequired();

            Property(a => a.Position)                   
                    .IsRequired();

            Property(a => a.IsActive)                    
                    .IsRequired();

            Property(a => a.DateCreated)                    
                    .IsRequired();

            Property(a => a.DateUpdated)                  
                    .IsRequired();

            Property(a => a.UpdatedBy)
                   .HasMaxLength(7)
                   .IsFixedLength()
                   .IsRequired();

            Property(a => a.CreatedBy)
                   .HasMaxLength(7)
                   .IsFixedLength()
                   .IsRequired();
        }
    }
}
