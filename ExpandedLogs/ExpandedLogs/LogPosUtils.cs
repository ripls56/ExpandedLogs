using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace ExpandedLogs
{
    /// <summary>
    /// Used to convert absolute coords to relative coords
    /// </summary>
    public static class LogPosUtils
    {
        public static ICoreServerAPI api;
        public static Vec3d AbsToRel(Vec3d abs)
        {
            var start = api.World.DefaultSpawnPosition.XYZ;
            abs.Sub(start.X, 0, start.Z);
            return abs;
        }

        public static Vec3i AbsToRel(Vec3i abs)
        {
            var start = api.World.DefaultSpawnPosition.XYZ.AsVec3i;
            abs.X -= start.X;
            abs.Z -= start.Z;
            return abs;
        }
    }
}
