using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace ExpandedLogs
{
    /// <summary>
    /// Used in logger instead of <see cref="Entity"/> because of shit ton of textures and other things
    /// </summary>
    class LogEntity
    {
        public EnumEntityState state;
        public EntityStats stats;
        public Vec3d cords;

        /// <summary>
        /// Just map function, map from ingame <see cref="Entity"/> to <see cref="LogEntity"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static LogEntity FromGameEntity(Entity entity)
        {

            return new LogEntity
            {
                state = entity.State,
                stats = entity.Stats,
                cords = LogPosUtils.AbsToRel(entity.Pos.XYZ)
            };
        }
    }
}
