using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace RR
{
    public class HistoryAutoRecorderWorker_ConsideredWealth : HistoryAutoRecorderWorker
    {
        public override float PullRecord()
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
			return num;
		}
    }
}