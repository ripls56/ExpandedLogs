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
        private EventManager eventManager;
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
            Console.WriteLine("[ExpandedLogs]: config loaded successfully. " + config);
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            eventManager = new(api);

            if (config.LogEvents.ShowAudit)
            {
                api.Logger.EntryAdded += eventManager.Logger_EntryAdded;
            }

            // Block events
            foreach (var logEvent in config.LogEvents.BlockEvents)
            {
                switch (logEvent)
                {
                    case "DidPlaceBlock":
                        api.Event.DidPlaceBlock += eventManager.Event_DidPlaceBlock;
                        break;
                    case "DidBreakBlock":
                        api.Event.DidBreakBlock += eventManager.Event_DidBreakBlock;
                        break;
                    case "DidUseBlock":
                        api.Event.DidUseBlock += eventManager.Event_DidUseBlock;
                        break;
                    case "BreakBlock":
                        api.Event.BreakBlock += eventManager.Event_BreakBlock;
                        break;
                }
            }

            // Player events
            foreach (var logEvent in config.LogEvents.PlayerEvents)
            {
                switch (logEvent)
                {
                    case "PlayerChat":
                        api.Event.PlayerChat += eventManager.Event_PlayerChat;
                        break;
                    case "PlayerDeath":
                        api.Event.PlayerDeath += eventManager.Event_PlayerDeath;
                        break;
                    case "PlayerRespawn":
                        api.Event.PlayerRespawn += eventManager.Event_PlayerRespawn;
                        break;
                    case "OnPlayerInteractEntity":
                        api.Event.OnPlayerInteractEntity += eventManager.Event_OnPlayerInteractEntity;
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
                        api.Event.ChunkColumnLoaded += eventManager.Event_ChunkColumnLoaded;
                        break;
                }
            }

            TimerCallback tm = new(eventManager.GetAllPlayersCoords);
            var delay = TimeSpan.FromSeconds(config.PlayerCoords.StartDelay);
            var interval = TimeSpan.FromSeconds(config.PlayerCoords.Interval);
            timer = new(tm, null, delay, interval);

            LogPosUtils.api = api;
        }

        public override void Dispose()
        {
            timer.Dispose();
            base.Dispose();
        }
    }
}