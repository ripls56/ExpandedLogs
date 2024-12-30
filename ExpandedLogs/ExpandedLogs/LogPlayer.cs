using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace ExpandedLogs
{
    /// <summary>
    /// Used in logger instead of <see cref="IPlayer"/> because of shit ton of textures and other things
    /// </summary>
    class LogPlayer
    {
        /// <summary>
        /// Player nickname
        /// </summary>
        public string nickname;

        /// <summary>
        /// Player unique id
        /// </summary>
        public string uid;

        /// <summary>
        /// World data
        /// </summary>
        public LogWorldData worldData;

        /// <summary>
        /// Just map function, map from ingame <see cref="IPlayer"/> to <see cref="LogPlayer"/>
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static LogPlayer FromGamePlayer(IPlayer player)
        {

            return new LogPlayer
            {
                nickname = player.PlayerName,
                uid = player.PlayerUID,
                worldData = LogWorldData.FromGameWorldData(player.WorldData, player.Entity.Pos.XYZ)
            };
        }
    }
}
