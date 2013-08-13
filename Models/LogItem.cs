using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingStone.Models
{
    public class LogItem
    {
        public LogTypes LogType { get; set; }
        public ActivityTypes ActivityType { get; set; }

        public string IPAddress { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public string ObjectID { get; set; }
        public string Email { get; set; }

    }
}
