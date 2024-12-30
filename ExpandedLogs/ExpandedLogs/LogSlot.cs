using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace ExpandedLogs
{
    /// <summary>
    /// Used in logger instead of <see cref="ItemSlot"/> because of shit ton of textures and other things
    /// </summary>
    class LogSlot
    {
        /// <summary>
        /// Slot number
        /// </summary>
        public string slot;

        /// <summary>
        /// Item name
        /// </summary>
        public string name;

        /// <summary>
        /// Stack size
        /// </summary>
        public int stackSize;

        /// <summary>
        /// Just map function, map from ingame <see cref="ItemSlot"/> to <see cref="LogSlot"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static LogSlot FromGameSlot(ItemSlot item)
        {
            return new LogSlot
            {
                slot = item.BackgroundIcon,
                name = SafeGetName(item),
                stackSize = item.StackSize,
            };
        }

        private static string SafeGetName(ItemSlot item)
        {
            var name = item.GetStackName();
            if (name == null)
            {
                name = "Empty";
            }
            return name;
        }
    }
}
