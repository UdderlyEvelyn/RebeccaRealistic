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
    public class MapComp_Realistic : MapComponent
    {
        public float ConsideredWealth = 0;
        public int LastUpdateTick = 0;

        public MapComp_Realistic(Map map) : base(map)
        {

        }

        public override void MapComponentTick()
        {
            var currentTick = Find.TickManager.TicksGame;
            var ticksSinceLastUpdate = currentTick - LastUpdateTick;
            if (ticksSinceLastUpdate > 60000)
            {
                var daysSinceLastUpdate = ticksSinceLastUpdate / 60000;
                Mathf.Lerp(ConsideredWealth, map.PlayerWealthForStoryteller, RebeccaSettings.DelayedWealthEffectPerDay * daysSinceLastUpdate);
                LastUpdateTick = currentTick;
            }
            base.MapComponentTick();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref ConsideredWealth, "ConsideredWealth");
            Scribe_Values.Look(ref LastUpdateTick, "LastUpdateTick");
            base.ExposeData();
        }
    }
}
