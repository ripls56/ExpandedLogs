using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;
using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using ExpandedLogs;

namespace ExpandedLogs
{
    public class ExpandedLogsModSystem : ModSystem
    {
        private ICoreServerAPI api;
        private Timer timer;
        private Config config;
        private const string configFileName = "expandedlogs.json";

        public override void Start(ICoreAPI api)
        {
            api.Logger.Notification("Expanded logs start");
            var config = api.LoadModConfig<Config>(configFileName);
            if (config == null)
            {
                api.Logger.Error("[ExpandedLogs]: expandedlogs.json not found in mod config folder.");
                api.Logger.Notification("[ExpandedLogs]: loaded default config");
                config = api.Assets.Get<Config>(new AssetLocation("expandedlogs", "config/" + configFileName));
            }
            this.config = config;
            api.Logger.Notification("[ExpandedLogs]: config loaded successfully. " + config);
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            if (config.LogEvents.ShowAudit)
            {
                api.Logger.EntryAdded += Logger_EntryAdded;
            }

            // Block events
            foreach (var logEvent in config.LogEvents.BlockEvents)
            {
                switch (logEvent)
                {
                    case "DidPlaceBlock":
                        api.Event.DidPlaceBlock += Event_DidPlaceBlock;
                        break;
                    case "DidBreakBlock":
                        api.Event.DidBreakBlock += Event_DidBreakBlock;
                        break;
                    case "DidUseBlock":
                        api.Event.DidUseBlock += Event_DidUseBlock;
                        break;
                    case "BreakBlock":
                        api.Event.BreakBlock += Event_BreakBlock;
                        break;
                }
            }

            // Player events
            foreach (var logEvent in config.LogEvents.PlayerEvents)
            {
                switch (logEvent)
                {
                    case "PlayerChat":
                        api.Event.PlayerChat += Event_PlayerChat;
                        break;
                    case "PlayerDeath":
                        api.Event.PlayerDeath += Event_PlayerDeath;
                        break;
                    case "PlayerRespawn":
                        api.Event.PlayerRespawn += Event_PlayerRespawn;
                        break;
                    case "OnPlayerInteractEntity":
                        api.Event.OnPlayerInteractEntity += Event_OnPlayerInteractEntity;
                        break;
                }
            }

            //api.Event.AfterActiveSlotChanged += Event_AfterActiveSlotChanged;

            // Chunk events
            foreach (var logEvent in config.LogEvents.PlayerEvents)
            {
                switch (logEvent)
                {
                    case "ChunkColumnLoaded":
                        api.Event.ChunkColumnLoaded += Event_ChunkColumnLoaded;
                        break;
                }
            }

            TimerCallback tm = new(GetAllPlayersCoords);
            var delay = TimeSpan.FromSeconds(config.PlayerCoords.StartDelay);
            var interval = TimeSpan.FromSeconds(config.PlayerCoords.Interval);
            timer = new(tm, null, delay, interval);

            this.api = api;
            LogPosUtils.api = api;
        }

        public override void Dispose()
        {
            timer.Dispose();
            base.Dispose();
        }

        private void GetAllPlayersCoords(object _)
        {
            api.World.AllPlayers.Foreach(p =>
            {
                Log("PlayerCoords", new Dictionary<string, object>
                {
                    { "player_name", p.PlayerName },
                    { "coords", LogPosUtils.AbsToRel(p.Entity.Pos.XYZ) },
                });
            });
        }

        private void Event_ChunkColumnLoaded(Vec2i chunkCoord, IWorldChunk[] chunks)
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

        private void Event_PlayerRespawn(IServerPlayer byPlayer)
        {
            Log("PlayerRespawn", new Dictionary<string, object>
            {
                { "player", LogPlayer.FromGamePlayer(byPlayer) }
            });
        }

        private void Event_PlayerDeath(IServerPlayer byPlayer, DamageSource damageSource)
        {
            Log("PlayerDeath", new Dictionary<string, object>
            {
                { "player", LogPlayer.FromGamePlayer(byPlayer) },
                { "damage_source", damageSource.Type }
            });
        }

        private void Event_PlayerChat(IServerPlayer byPlayer, int channelId, ref string message, ref string data, Vintagestory.API.Datastructures.BoolRef consumed)
        {
            Log("PlayerChat", new Dictionary<string, object>
            {
                { "player", LogPlayer.FromGamePlayer(byPlayer) },
                { "channel_id", channelId },
                { "message", message },
                { "data", data }
            });
        }

        private void Event_OnPlayerInteractEntity(Vintagestory.API.Common.Entities.Entity entity, IPlayer byPlayer, ItemSlot slot, Vintagestory.API.MathTools.Vec3d hitPosition, int mode, ref EnumHandling handling)
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

        private void Event_DidUseBlock(IServerPlayer byPlayer, BlockSelection blockSel)
        {
            Log("DidUseBlock", new Dictionary<string, object>
            {
                { "player", LogPlayer.FromGamePlayer(byPlayer) },
                { "block_selection", LogPosUtils.AbsToRel(blockSel.Position.AsVec3i) }
            });
        }

        private void Event_DidPlaceBlock(IServerPlayer byPlayer, int oldblockId, BlockSelection blockSel, ItemStack stack)
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

        private void Event_BreakBlock(IServerPlayer byPlayer, BlockSelection blockSel, ref float dropQuantityMultiplier, ref EnumHandling handling)
        {
            Log("BreakBlock", new Dictionary<string, object>
            {
                { "player", LogPlayer.FromGamePlayer(byPlayer) },
                { "block_selection", LogPosUtils.AbsToRel(blockSel.Position.AsVec3i) },
                { "drop_quantity_multiplier", dropQuantityMultiplier }
            });
        }

        private void Event_DidBreakBlock(IServerPlayer byPlayer, int oldblockId, BlockSelection blockSel)
        {
            Log("DidBreakBlock", new Dictionary<string, object>
            {
                { "player", LogPlayer.FromGamePlayer(byPlayer) },
                { "old_block_id", oldblockId },
                { "block_selection", LogPosUtils.AbsToRel(blockSel.Position.AsVec3i) }
            });
        }

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

        private void Logger_EntryAdded(EnumLogType logType, string message, params object[] args)
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
                        string result = Regex.Replace(message, pattern, match =>
                        {
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
    }
}