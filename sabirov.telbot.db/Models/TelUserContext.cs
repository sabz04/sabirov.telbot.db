using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sabirov.telbot.db.Models
{
    public class TelUserContext:DbContext
    {
        public TelUserContext():
            base("DBConnection")
        {

        }
        public DbSet<TelUser> telUsers { get; set; }
    }
}
