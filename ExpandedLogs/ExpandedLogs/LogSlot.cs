using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace ExpandedLogs
{
    class LogSlot
    {
        public string slot;
        public string name;
        public int stackSize;

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
