using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace ExpandedLogs
{
    class LogEntity
    {
        public EnumEntityState state;
        public EntityStats stats;
        public Vec3d cords;

        public static LogEntity FromGameEntity(Vintagestory.API.Common.Entities.Entity entity)
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
