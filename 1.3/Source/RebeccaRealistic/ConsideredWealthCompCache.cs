using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace RR
{
    //This stores the comp on a per-map basis so that patches don't have to constantly retrieve it.
    public static class ConsideredWealthCompCache
    {
        public static Dictionary<int, MapComponent_ConsideredWealth> compCachePerMap = new Dictionary<int, MapComponent_ConsideredWealth>();

        public static MapComponent_ConsideredWealth GetFor(Map map)
        {
            MapComponent_ConsideredWealth comp; //Set up var.
            if (!compCachePerMap.ContainsKey(map.uniqueID)) //If not cached..
                compCachePerMap.Add(map.uniqueID, comp = map.GetComponent<MapComponent_ConsideredWealth>()); //Get and cache.
            else
                comp = compCachePerMap[map.uniqueID]; //Retrieve from cache.
            return comp;
        }

        public static void SetFor(Map map, MapComponent_ConsideredWealth comp)
        {
            if (!compCachePerMap.ContainsKey(map.uniqueID)) //If not cached..
                compCachePerMap.Add(map.uniqueID, comp); //Cache.
            else
                compCachePerMap[map.uniqueID] = comp; //Reassign cache.
        }
    }
}
