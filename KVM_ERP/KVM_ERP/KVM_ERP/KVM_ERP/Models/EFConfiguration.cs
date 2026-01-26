using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace ClubMembership.Models
{
    // Enables retry-on-failure for transient SQL errors (e.g., SQL Azure / flaky networks)
    public class EFConfiguration : DbConfiguration
    {
        public EFConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
        }
    }
}


