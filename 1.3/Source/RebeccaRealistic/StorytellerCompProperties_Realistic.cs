using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace RR
{
	public class StorytellerCompProperties_Realistic : StorytellerCompProperties
	{
		public float mtbDays;

		public List<IncidentCategoryEntry> categoryWeights = new List<IncidentCategoryEntry>();

		public float maxThreatBigIntervalDays = 99999f;

		public FloatRange randomPointsFactorRange = new FloatRange(0.5f, 1.5f);

		public bool skipThreatBigIfRaidBeacon;

		public StorytellerCompProperties_Realistic()
		{
			compClass = typeof(StorytellerComp_Realistic);
		}
	}
}
