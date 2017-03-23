using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicy.UserManagement.Server.DataAccess.Configurations
{
    public class VicyDbConfiguration : DbConfiguration
    {
        public VicyDbConfiguration(
            DbConnectionStrings connnectionStrings,
            PoorPerformingSqlLogger poorPerformingSqlLogger)
        {
            if (connnectionStrings == null)
                throw new ArgumentNullException(nameof(connnectionStrings));

            AddInterceptor(poorPerformingSqlLogger);

            // Overwrite the connection factory so it create connection 
            // based on our connectionstring from configuraiton.
            Loaded +=
                (sender, args) =>
                {
                    args.ReplaceService<IDbConnectionFactory>((s, _) =>
                        new VicyDbConnectionFactory(connnectionStrings));
                };
        }

        protected VicyDbConfiguration()
        {
        }
    }
}
