using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace ExpandedLogs
{
    class LogPlayer
    {
        public string nickname;
        public string uid;
        public LogWorldData worldData;

        public static LogPlayer FromGamePlayer(IPlayer player)
        {

            return new LogPlayer
            {
                nickname = player.PlayerName,
                uid = player.PlayerUID,
                worldData = LogWorldData.FromGamePlayer(player.WorldData, player.Entity.Pos.XYZ)
            };
        }
    }
}
