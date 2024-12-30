using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace ExpandedLogs
{
    /// <summary>
    /// Used in logger instead of <see cref="IWorldPlayerData"/> because of shit ton of textures and other things
    /// </summary>
    class LogWorldData
    {
        public string playeruid;
        public int deaths;
        public Vec3d coords;

        /// <summary>
        /// Just map function, map from ingame <see cref="IWorldPlayerData"/> to <see cref="LogWorldData"/>
        /// </summary>
        /// <param name="world"></param>
        /// <param name="cords"></param>
        /// <returns></returns>
        public static LogWorldData FromGameWorldData(IWorldPlayerData world, Vec3d cords)
        {
            return new LogWorldData
            {
                playeruid = world.PlayerUID,
                deaths = world.Deaths,
                coords = LogPosUtils.AbsToRel(cords)
            };
        }
    }
}
