using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace RR
{
    public class WorldComponent_ConsideredWealth : MapComponent
    {
        public static WorldComponent_ConsideredWealth Instance;

        public float ConsideredWealth = 0;
        public int LastUpdateTick = 0;
        public bool Initialized = false;

        public WorldComponent_ConsideredWealth(Map map) : base(map)
        {
            Log.Message("[Rebecca Realistic] New WorldComponent_ConsideredWealth constructed (for map " + map.uniqueID + ") adding it to the cache.");
            Instance = this;
        }

        public override void MapGenerated()
        {
            Initialized = true; //Initialize on map generation so that we only fire the code to shove considered wealth up to 95% of total on loaded pre-existing saves.
        }

        public override void MapComponentTick()
        {
            var currentTick = Find.TickManager.TicksGame;
            var ticksSinceLastUpdate = currentTick - LastUpdateTick;
            //It's been more than a day since last update, or last update hasn't happened and we're more than a day in (e.g., swapped storyteller to Rebecca).
            if (ticksSinceLastUpdate > 60000)
            {
                float num = 0f;
                List<Map> maps = Find.Maps;
                for (int i = 0; i < maps.Count; i++)
                {
                    if (maps[i].IsPlayerHome)
                    {
                        num += ConsideredWealthCompCache.GetFor(maps[i]).ConsideredWealth;
                    }
                }
                ConsideredWealth = num;
                LastUpdateTick = currentTick;
                Initialized = true; //Prevent expensive math in above safeguard, bool check is cheap.
            }
            base.MapComponentTick();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref ConsideredWealth, "ConsideredWealth");
            Scribe_Values.Look(ref LastUpdateTick, "LastUpdateTick");
            Scribe_Values.Look(ref Initialized, "Initialized");
            base.ExposeData();
        }
    }
}
