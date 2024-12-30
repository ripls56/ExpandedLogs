using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace ExpandedLogs
{
    class LogWorldData
    {
        public string playeruid;
        public int deaths;
        public Vec3d cords;

        public static LogWorldData FromGamePlayer(IWorldPlayerData world, Vec3d cords)
        {
            return new LogWorldData
            {
                playeruid = world.PlayerUID,
                deaths = world.Deaths,
                cords = LogPosUtils.AbsToRel(cords)
            };
        }
    }
}
