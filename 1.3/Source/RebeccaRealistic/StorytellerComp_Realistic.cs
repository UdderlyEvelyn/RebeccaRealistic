using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;
using RimWorld.Planet;

namespace RR
{
	public class StorytellerComp_Realistic : StorytellerComp
    {
		protected IncidentCategoryDef iCatDefThreatBig = DefDatabase<IncidentCategoryDef>.GetNamed("ThreatBig");
		protected IncidentCategoryDef iCatDefOrbitalVisitor = DefDatabase<IncidentCategoryDef>.GetNamed("OrbitalVisitor");
		protected IncidentCategoryDef iCatDefFactionArrival = DefDatabase<IncidentCategoryDef>.GetNamed("FactionArrival");
		protected FiringIncident[] blankList = new FiringIncident[0];

		protected StorytellerCompProperties_Realistic Props => (StorytellerCompProperties_Realistic)props;

		public void RebeccaLog(string message)
        {
			if (RebeccaSettings.LoggingEnabled)
				Log.Message("[UdderlyEvelyn.RebeccaRealistic][" + DateTime.Now.ToShortTimeString() + "] " + message);
		}

		public override IEnumerable<FiringIncident> MakeIntervalIncidents(IIncidentTarget target)
		{
			if (!Rand.MTBEventOccurs(Props.mtbDays, RebeccaSettings.MTBUnit, 1000f))
			{
				RebeccaLog("Rebecca has decided to send nothing..");
				return blankList;
			}
			else if (target == null) //Problem.
            {
				RebeccaLog("Rebecca wanted to send something but the target was null. :(");
				return blankList;
			}
			//Added chance of an additional event of ThreatBig.
			if (RebeccaSettings.LoggingEnabled) //Double-check this one since it does some math, no need to do the math if we're not logging.
				RebeccaLog("Rebecca is considering sending an extra ThreatBig, chance is: " + Math.Round(100 * (RebeccaSettings.BaseBonusThreatBigChance + (RebeccaSettings.BonusThreatBigChancePerWealthChance * (target.PlayerWealthForStoryteller / RebeccaSettings.BonusThreatBigChancePerWealthThreshold))), 2) + "%");
			if (Rand.Chance(RebeccaSettings.BaseBonusThreatBigChance + (RebeccaSettings.BonusThreatBigChancePerWealthChance * (target.PlayerWealthForStoryteller / RebeccaSettings.BonusThreatBigChancePerWealthThreshold)))) //10% chance of big incident tacked on plus another 10% per 60k wealth.
				SendRandomWeightedIncidentFromCategory(iCatDefThreatBig, target, 2500, 3750); //Slightly larger window so it doesn't stack with the visitors or the other too closely.
			else
				RebeccaLog("Rebecca decided not to.");
			//Shunt off the visitors to not take up valuable incident randomness since there's so many of those events..
			if (Rand.Chance(RebeccaSettings.VisitorChance)) //15% chance of having a visitor of some sort
            {
				if (Rand.Chance(RebeccaSettings.VisitorIsOrbitalChance)) //46% chance of it being an orbital visitor (derived from comparing Randy weights for OrbitalVisitor and FactionArrival)
					SendRandomWeightedIncidentFromCategory(iCatDefOrbitalVisitor, target);
				else //FactionArrival instead
					SendRandomWeightedIncidentFromCategory(iCatDefFactionArrival, target);
			}
			//The rest
			var iCatDef = Props.categoryWeights.RandomElementByWeight(cw => cw.weight).category;
			return new FiringIncident[] { GetRandomWeightedIncidentFromCategory(iCatDef, target) };
		}

		public void SendRandomWeightedIncidentFromCategory(IncidentCategoryDef iCatDef, IIncidentTarget target, int minTicks = 1250, int maxTicks = 2500)
        {
			Current.Game.storyteller.incidentQueue.Add(new QueuedIncident(GetRandomWeightedIncidentFromCategory(iCatDef, target), Find.TickManager.TicksGame + Rand.Range(minTicks, maxTicks)));
		}

		public FiringIncident GetRandomWeightedIncidentFromCategory(IncidentCategoryDef iCatDef, IIncidentTarget target)
        {
			var parms = GenerateParms(iCatDef, target);
			var foundDef = UsableIncidentsInCategory(iCatDef, parms).RandomElementByWeightWithFallback(i => i.Worker.BaseChanceThisGame);
			RebeccaLog("Rebecca is sending \"" + foundDef.defName + "\" from category \"" + iCatDef.defName + "\".");
			return new FiringIncident(foundDef, this, parms);
		}

		//Copypasta from Randy's RandomMain but pointing at our method for defaultParmsNow
		public override IncidentParms GenerateParms(IncidentCategoryDef incCat, IIncidentTarget target)
		{
			IncidentParms incidentParms = defaultParmsNow(incCat, target);
			if (incidentParms.points >= 0f)
			{
				incidentParms.points *= Props.randomPointsFactorRange.RandomInRange;
			}
			return incidentParms;
		}

		//Copypasta from StorytellerUtility but pointing at our method for defaultThreatPointsNow.
		protected static IncidentParms defaultParmsNow(IncidentCategoryDef incCat, IIncidentTarget target)
		{
			if (incCat == null)
			{
				Log.Warning("Trying to get default parms for null incident category.");
			}
			IncidentParms incidentParms = new IncidentParms();
			incidentParms.target = target;
			if (incCat.needsParmsPoints)
			{
				incidentParms.points = defaultThreatPointsNow(target);
			}
			return incidentParms;
		}

		//Copypasta from StorytellerUtility
		protected static readonly SimpleCurve pointsPerWealthCurve = new SimpleCurve
		{
			new CurvePoint(0f, 0f),
			new CurvePoint(14000f, 0f),
			new CurvePoint(400000f, 2400f),
			new CurvePoint(700000f, 3600f),
			new CurvePoint(1000000f, 4200f)
		};

		//Copypasta from StorytellerUtility w/ modifications to yeet features we don't want.
		protected static float defaultThreatPointsNow(IIncidentTarget target)
		{
			float playerWealthForStoryteller = target.PlayerWealthForStoryteller;
			float num = pointsPerWealthCurve.Evaluate(playerWealthForStoryteller);
			float num2 = 0f;
			foreach (Pawn item in target.PlayerPawnsForStoryteller)
			{
				if (item.IsQuestLodger())
				{
					continue;
				}
				float num3 = 0f;
				if (item.RaceProps.Animal && item.Faction == Faction.OfPlayer && !item.Downed && item.training.CanAssignToTrain(TrainableDefOf.Release).Accepted)
				{
					num3 = 0.08f * item.kindDef.combatPower;
					if (target is Caravan)
					{
						num3 *= 0.7f;
					}
				}
				if (num3 > 0f)
				{
					if (item.ParentHolder != null && item.ParentHolder is Building_CryptosleepCasket)
					{
						num3 *= 0.3f;
					}
					num3 = Mathf.Lerp(num3, num3 * item.health.summaryHealth.SummaryHealthPercent, 0.65f);
					num2 += num3;
				}
			}
			float num4 = (num + num2) * target.IncidentPointsRandomFactorRange.RandomInRange;
			//Yeet kindness.
			//float totalThreatPointsFactor = Find.StoryWatcher.watcherAdaptation.TotalThreatPointsFactor;
			//float num5 = Mathf.Lerp(1f, totalThreatPointsFactor, Find.Storyteller.difficulty.adaptationEffectFactor);
			return Mathf.Clamp(num4 /** num5*/ * Find.Storyteller.difficulty.threatScale /** Find.Storyteller.def.pointsFactorFromDaysPassed.Evaluate(GenDate.DaysPassedSinceSettle)*/, 35f, 10000f);
		}
	}
}
