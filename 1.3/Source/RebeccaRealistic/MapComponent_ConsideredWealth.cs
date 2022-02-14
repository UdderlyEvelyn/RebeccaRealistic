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
    public class MapComponent_ConsideredWealth : MapComponent
    {
        public float ConsideredWealth = 0;
        public int LastUpdateTick = 0;
        public bool Initialized = false;

        public MapComponent_ConsideredWealth(Map map) : base(map)
        {
            Log.Message("[Rebecca Realistic] New MapComponent_ConsideredWealth constructed (for map " + map.uniqueID + ") adding it to the cache.");
            ConsideredWealthCompCache.SetFor(map, this);
        }

        public override void MapRemoved()
        {
            Log.Message("[Rebecca Realistic] Removing MapComponent_ConsideredWealth from cache due to map removal (for map " + map.uniqueID + ").");
            ConsideredWealthCompCache.compCachePerMap.Remove(map.uniqueID); //Yeet from cache so it can die in the GC.
        }

        public override void MapGenerated()
        {
            Initialized = true; //Initialize on map generation so that we only fire the code to shove considered wealth up to 95% of total on loaded pre-existing saves.
        }

        public override void MapComponentTick()
        {
            var currentTick = Find.TickManager.TicksGame;
            var ticksSinceLastUpdate = currentTick - LastUpdateTick;
            if (!Initialized && (LastUpdateTick == 0 && currentTick > 60000)) //This should only fire if Rebecca is enabled on a save that didn't have her before.
            {
                ConsideredWealth = map.PlayerWealthForStoryteller * .95f; //put 95% of the wealth as known, assume there's a bit of new to be nice.
                Initialized = true; //Stop it from firing so we don't do unnecessary math each tick, bool check is cheap.
            }
            //It's been more than a day since last update, or last update hasn't happened and we're more than a day in (e.g., swapped storyteller to Rebecca).
            if (ticksSinceLastUpdate > 60000)
            {
                var daysSinceLastUpdate = ticksSinceLastUpdate / 60000;
                ConsideredWealth = Mathf.Lerp(ConsideredWealth, map.PlayerWealthForStoryteller, RebeccaSettings.DelayedWealthEffectPerDay * daysSinceLastUpdate);
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
