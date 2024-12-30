using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpandedLogs
{
    public class Config
    {
        public PlayerCords PlayerCoords { get; set; }

        public LogEvents LogEvents { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }

    public class PlayerCords
    {
        public int Interval { get; set; }

        public int StartDelay { get; set; }
    }

    public class LogEvents
    {
        public bool ShowAudit { get; set; }

        public List<string> PlayerEvents { get; set; }

        public List<string> BlockEvents { get; set; }

        public List<string> ChunkEvents { get; set; }
    }
}
