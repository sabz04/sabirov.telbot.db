using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sabirov.telbot.db.Models
{
    public class TelUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long ChatId { get; set; }
        public byte[] Photo { get; set; }

    }
}
