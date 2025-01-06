using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace ExpandedLogs
{
    public class EventManager
    {
        private ICoreServerAPI api;

        public EventManager(ICoreServerAPI api)
        {
            this.api = api;
        }

        /// <summary>
        /// Get all player coords
        /// </summary>
        /// <param name="_"></param>
        public void GetAllOnlinePlayersCoords(object _)
        {
            api.World.AllOnlinePlayers.Foreach(p =>
            {
                var scope = "PlayerCoords";
                if (p == null) {
                    Console.WriteLine($"[Warning] [ExpandedLog] [{scope}] player is null");
                    return;
                }
                if (p?.PlayerName == null) {
                    Console.WriteLine($"[Warning] [ExpandedLog] [{scope}] player name is null");
                    return;
                }
                if (p?.Entity == null) {
                    Console.WriteLine($"[Warning] [ExpandedLog] [{scope}] entity pos is null");
                    return;
                }
                if (p?.Entity?.Pos == null) {
                    Console.WriteLine($"[Warning] [ExpandedLog] [{scope}] entity pos is null");
                    return;
                }

                var pos = LogPosUtils.AbsToRel(p.Entity.Pos.XYZ);
                if (pos.AsVec3i == api.World.DefaultSpawnPosition.XYZ.AsVec3i) { return; }
                Log(scope, new Dictionary<string, object>
                {
                    { "player_name", p.PlayerName },
                    { "coords", pos },
                });
            });
        }

        public void Event_ChunkColumnLoaded(Vec2i chunkCoord, IWorldChunk[] chunks)
        {
            Log("ChunkColumnLoaded", new Dictionary<string, object>
            {
                { "chunk_coord", chunkCoord },
                { "chunks_loaded", chunks.Length }
            });
        }

        //private void Event_AfterActiveSlotChanged(IServerPlayer byPlayer, ActiveSlotChangeEventArgs activeSlot)
        //{
        //    Log("AfterActiveSlotChanged", new Dictionary<string, object>
        //    {
        //        { "player", LogPlayer.FromGamePlayer(byPlayer) },
        //        { "active_slot", activeSlot }
        //    });
        //}

        public void Event_PlayerRespawn(IServerPlayer byPlayer)
        {
            Log("PlayerRespawn", new Dictionary<string, object>
            {
                { "player", LogPlayer.FromGamePlayer(byPlayer) }
            });
        }

        public void Event_PlayerDeath(IServerPlayer byPlayer, DamageSource damageSource)
        {
            Log("PlayerDeath", new Dictionary<string, object>
            {
                { "player", LogPlayer.FromGamePlayer(byPlayer) },
                { "damage_source", Enum.GetName(damageSource.Type) }
            });
        }

        public void Event_PlayerChat(IServerPlayer byPlayer, int channelId, ref string message, ref string data, Vintagestory.API.Datastructures.BoolRef consumed)
        {
            Log("PlayerChat", new Dictionary<string, object>
            {
                { "player", LogPlayer.FromGamePlayer(byPlayer) },
                { "channel_id", channelId },
                { "message", message },
                { "data", data }
            });
        }

        public void Event_OnPlayerInteractEntity(Vintagestory.API.Common.Entities.Entity entity, IPlayer byPlayer, ItemSlot slot, Vintagestory.API.MathTools.Vec3d hitPosition, int mode, ref EnumHandling handling)
        {
            Log("OnPlayerInteractEntity", new Dictionary<string, object>
            {
                { "entity", LogEntity.FromGameEntity(entity) },
                { "player", LogPlayer.FromGamePlayer(byPlayer) },
                { "inventory", LogSlot.FromGameSlot(slot) },
                { "hit_position", hitPosition },
                { "mode", mode }
            });
        }

        public void Event_DidUseBlock(IServerPlayer byPlayer, BlockSelection blockSel)
        {
            Log("DidUseBlock", new Dictionary<string, object>
            {
                { "player", LogPlayer.FromGamePlayer(byPlayer) },
                { "block_selection", LogPosUtils.AbsToRel(blockSel.Position.AsVec3i) }
            });
        }

        public void Event_DidPlaceBlock(IServerPlayer byPlayer, int oldblockId, BlockSelection blockSel, ItemStack stack)
        {
            Log("DidPlaceBlock", new Dictionary<string, object>
            {
                { "player", LogPlayer.FromGamePlayer(byPlayer) },
                { "old_block_id", oldblockId },
                { "block_selection", LogPosUtils.AbsToRel(blockSel.Position.AsVec3i) },
                { "item", new Dictionary<string, object>
                {
                    { "count", stack.StackSize },
                    { "id", stack.Id},
                    { "code", stack.Collectible?.Code },
                } }
            });
        }

        public void Event_BreakBlock(IServerPlayer byPlayer, BlockSelection blockSel, ref float dropQuantityMultiplier, ref EnumHandling handling)
        {
            Log("BreakBlock", new Dictionary<string, object>
            {
                { "player", LogPlayer.FromGamePlayer(byPlayer) },
                { "block_selection", LogPosUtils.AbsToRel(blockSel.Position.AsVec3i) },
                { "drop_quantity_multiplier", dropQuantityMultiplier }
            });
        }

        public void Event_DidBreakBlock(IServerPlayer byPlayer, int oldblockId, BlockSelection blockSel)
        {
            Log("DidBreakBlock", new Dictionary<string, object>
            {
                { "player", LogPlayer.FromGamePlayer(byPlayer) },
                { "old_block_id", oldblockId },
                { "block_selection", LogPosUtils.AbsToRel(blockSel.Position.AsVec3i) }
            });
        }

        public void Logger_EntryAdded(EnumLogType logType, string message, params object[] args)
        {
            try
            {
                switch (logType)
                {
                    case EnumLogType.Audit:
                        if (args == null)
                        {
                            Console.WriteLine("[ExpandedLogs] разраб насрал и не убрал");
                            return;
                        }
                        string pattern = @"\{(\d+)\}";

                        // Use regex because message look like: {0} some message. Where {0} index in args
                        string result = Regex.Replace(message, pattern, match =>
                        {
                            if (match.Groups[1].Value == null) { return message; }
                            int index = int.Parse(match.Groups[1].Value);
                            return args[index].ToString();
                        });
                        Console.WriteLine($"[Audit] {result}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ExpandedLogs] " + ex);
            }
        }

        /// <summary>
        /// Helper function for logging as in production
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="param"></param>
        private void Log(string scope, Dictionary<string, object> param)
        {
            try
            {
                var sb = new StringBuilder();
                param.Add("scope", scope);
                var json = JsonConvert.SerializeObject(param, Formatting.None, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    MaxDepth = 1
                });
                sb.Append($"{json}");
                api.Logger.Audit($"[ExpandedLogs] {sb}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ExpandedLogs] " + ex);
            }
        }
    }
}
