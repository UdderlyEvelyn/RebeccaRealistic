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

        public override void Initialize()
        {
			RebeccaLog("Rebecca is waking up from a nap - she dreamed about a baby cow.");
			foreach (QueuedIncident qi in Current.Game.storyteller.incidentQueue)
				RebeccaLog("Rebecca has a \"" + qi.FiringIncident.def.defName + "\" firing in " + (qi.FireTick - Find.TickManager.TicksGame) + " ticks.");
            base.Initialize();
        }

        public static void RebeccaLog(string message)
        {
			//Log.ResetMessageCount();
			if (RebeccaSettings.LoggingEnabled)
				Log.Message("[Rebecca Realistic] - " + DateTime.Now.ToShortTimeString() + ": " + message);
		}

		public override IEnumerable<FiringIncident> MakeIntervalIncidents(IIncidentTarget target)
		{
			if (!Rand.MTBEventOccurs(Props.mtbDays, RebeccaSettings.MTBUnit, 1000f))
			{
				if (RebeccaSettings.LogNothingsEnabled)
					RebeccaLog("Rebecca has decided to send nothing..");
				return blankList;
			}
			else if (target == null) //Problem.
            {
				RebeccaLog("Rebecca wanted to send something but the target was null. :(");
				return blankList;
			}
			//The rest
			var iCatDef = Props.categoryWeights.RandomElementByWeight(cw => cw.weight).category;
			if (iCatDef == null)
			{
				RebeccaLog("Rebecca tried to find a category to send from, but somehow ended up with a null - damn it, Rebecca.");
				return blankList;
			}
			var incident = GetRandomWeightedIncidentFromCategory(iCatDef, target); //This will be sent at the end, but now we can check it during other things.
			bool poolIncidentIsRaid = incident == null ? false : (incident.def.Worker is IncidentWorker_RaidEnemy || (incident.def.tags != null && incident.def.tags.Contains("Raid"))); //It's a raid, with option to be supported if you don't use the class as a third party..
			if (incident != null) //Only calculate threatpoints if incident is valid.
				incident.parms.points = defaultThreatPointsNow(target, incident.def); //Add in the *real* points now that the incident has been selected.

			//Added chance of an additional incident of ThreatBig.
			if (poolIncidentIsRaid)
				RebeccaLog("Rebecca noticed the incident coming from the pool is a raid.. Chance for two ThreatBigs at once is " + Math.Round(100 * RebeccaSettings.TwoAtOnceThreatBigChance) + "%, rolling!");
			if (!poolIncidentIsRaid || (poolIncidentIsRaid && Rand.Chance(RebeccaSettings.TwoAtOnceThreatBigChance)))			{
				if (poolIncidentIsRaid)
					RebeccaLog("Rebecca decided you needed another ThreatBig at the same time - isn't she thoughtful?");
				var bonusBigThreatChance = (RebeccaSettings.BaseBonusThreatBigChance + (RebeccaSettings.BonusThreatBigChancePerWealthChance * (wealthForTarget(target) / RebeccaSettings.BonusThreatBigChancePerWealthThreshold)));
					RebeccaLog("Rebecca is considering sending an extra ThreatBig, chance is: " + Math.Round(100 * bonusBigThreatChance, 2) + "%");
				if (Rand.Chance(bonusBigThreatChance)) //10% chance of big incident tacked on plus another 10% per 10k wealth.
					SendRandomWeightedIncidentFromCategory(iCatDefThreatBig, target, RebeccaSettings.BonusThreatBigMinimumSpacingTicks, RebeccaSettings.BonusThreatBigMaximumSpacingTicks); //Slightly larger window so it doesn't stack with the visitors or the other too closely.
				else
					RebeccaLog("Rebecca decided not to.");
			}
			else
				RebeccaLog("Rebecca decided one ThreatBig was enough - she can be nice once in a while, savor it.");

			//Shunt off the visitors to not take up valuable incident randomness since there's so many of those events..
			if (Rand.Chance(RebeccaSettings.VisitorChance)) //15% chance of having a visitor of some sort
            {
				if (Rand.Chance(RebeccaSettings.VisitorIsOrbitalChance)) //46% chance of it being an orbital visitor (derived from comparing Randy weights for OrbitalVisitor and FactionArrival)
					SendRandomWeightedIncidentFromCategory(iCatDefOrbitalVisitor, target, RebeccaSettings.VisitorMinimumSpacingTicks, RebeccaSettings.VisitorMaximumSpacingTicks);
				else //FactionArrival instead
					SendRandomWeightedIncidentFromCategory(iCatDefFactionArrival, target, RebeccaSettings.VisitorMinimumSpacingTicks, RebeccaSettings.VisitorMaximumSpacingTicks);
			}

			if (incident == null) //No usable incident was found for pool incident..
				return blankList; //Abort.
			RebeccaLog("Rebecca is sending \"" + incident.def.defName + "\" from the pool right now!");
			return new FiringIncident[] { incident };
		}

		public void SendRandomWeightedIncidentFromCategory(IncidentCategoryDef iCatDef, IIncidentTarget target, int minTicks = 1250, int maxTicks = 2500)
        {
			var incident = GetRandomWeightedIncidentFromCategory(iCatDef, target);
			if (incident == null) //No usable incident was found..
				return; //Abort.
			incident.parms.points = defaultThreatPointsNow(target, incident.def); //Add in the *real* points now that the incident has been selected.
			var firingDelay = Rand.Range(minTicks, maxTicks);
			RebeccaLog("Rebecca is queuing \"" + incident.def.defName + "\" for " + firingDelay + " ticks from now.");
			Current.Game.storyteller.incidentQueue.Add(new QueuedIncident(incident, Find.TickManager.TicksGame + firingDelay, 2500));
		}

		public FiringIncident GetRandomWeightedIncidentFromCategory(IncidentCategoryDef iCatDef, IIncidentTarget target)
        {
			var parms = GenerateParms(iCatDef, target);
			RebeccaLog("Rebecca made the incident parameters, headpats for Rebecca!");
			//Evaluate the chance by pop curve as though you always have three colonists, giving it a "medium" sort of chance.
			IncidentDef foundDef = null;
			bool gotIncident = false;
			var categoryIncidents = UsableIncidentsInCategory(iCatDef, parms);
			if (categoryIncidents.Count() == 0)
            {
				RebeccaLog("Rebecca found no incidents for category \"" + iCatDef.defName + "\", so she will have to fire a null incident which will do nothing!");
				return null;
            }
			while (!gotIncident)
				gotIncident = categoryIncidents.TryRandomElementByWeight(i =>
				{
					if (i == null)
						RebeccaLog("Rebecca is considering a null incident, oh jeez.. stop that!");
					if (i.Worker == null)
						RebeccaLog("Rebecca found an incident with a null worker - that's weird! It's \"" + i.defName + "\".. Let someone know!");
					var popChance = 1f;
					switch (i.populationEffect)
					{
						case IncidentPopulationEffect.IncreaseEasy:
							popChance = .50f;
							break;
						case IncidentPopulationEffect.IncreaseMedium:
							popChance = .33f;
							break;
						case IncidentPopulationEffect.IncreaseHard:
							popChance = .25f;
							break;
					}
					var finalChance = popChance * i.Worker.BaseChanceThisGame;
					if (RebeccaSettings.IncidentSelectionLoggingEnabled)
						RebeccaLog("Rebecca is considering sending \"" + i.defName + "\", with" + (popChance != 1 ? " population chance compensation of " + popChance + " and" : "") + " a base chance of " + i.Worker.BaseChanceThisGame + " for a final chance of " + finalChance + "..");
					return finalChance;
				}, out foundDef);
			RebeccaLog("Rebecca has selected \"" + foundDef.defName + "\" from category \"" + iCatDef.defName + "\".");
			return new FiringIncident(foundDef, this, parms);
		}

		public override IncidentParms GenerateParms(IncidentCategoryDef incCat, IIncidentTarget target)
		{
			if (incCat == null)
			{
				Log.Warning("Trying to get default parms for null incident category.");
			}
			//Psst.. the points are fake, they will be replaced later!
			return new IncidentParms { target = target, points = 35 };
		}

		public static float wealthForTarget(IIncidentTarget target)
        {
			float wealth = 0;
			if (target is Map map) //If it's a map..
				wealth = ConsideredWealthCompCache.GetFor(map).ConsideredWealth;
			else //If it's not a map (what is it? lol..)
			{
				wealth = target.PlayerWealthForStoryteller;
				RebeccaLog("Rebecca found a non-Map target, it's a \"" + target.GetType().FullName + "\"!");
			}
			return wealth;
		}

		//Copypasta from StorytellerUtility w/ modifications to yeet features we don't want.
		protected static float defaultThreatPointsNow(IIncidentTarget target, IncidentDef rollingForIncidentDef)
		{
			RebeccaLog("Rebecca is calculating defaultThreatPointsNow for incident \"" + rollingForIncidentDef.defName + "\" with worker \"" + (rollingForIncidentDef.Worker == null ? "NULL" : rollingForIncidentDef.Worker.GetType().FullName) + "\".");
			bool rollingForRaid = rollingForIncidentDef.Worker is IncidentWorker_RaidEnemy || 
								  (rollingForIncidentDef.tale != null && rollingForIncidentDef.tale.defName == "Raid");
			bool rollingForManhunters = rollingForIncidentDef.Worker is IncidentWorker_ManhunterPack;
			bool rollingForInfestation = rollingForIncidentDef.Worker is IncidentWorker_Infestation || 
										 rollingForIncidentDef.Worker.def.defName == "IncidentWorker_BlackHive" ||
										 rollingForIncidentDef.Worker.def.defName == "BI_InsectRaid" ||
										 (rollingForIncidentDef.tale != null && rollingForIncidentDef.tale.defName == "Infestation");
			bool rollingForNeutralGroup = rollingForIncidentDef.Worker is IncidentWorker_NeutralGroup;
			//Doesn't matter what we set here, but in case we ever get to the end without setting it for a raid,
			//raids will error at $0 though work fine (there's a fallback system apparently), this avoids that just in case.
			float basePoints = Mathf.Max(rollingForIncidentDef.minThreatPoints, 35); 
			if (rollingForManhunters)
				basePoints = 50 * target.PlayerPawnsForStoryteller.Count(p =>
				{
					if (!(p.IsColonist || p.IsPrisoner || p.IsSlave))
						return false;
					if (p.ParentHolder != null && p.ParentHolder is Building_CryptosleepCasket) //Animals don't know people are in there.
						return false;
					return true;
				}); //50 points per non-stasis human-like since they're *manhunters*.
			else if (rollingForInfestation)
				basePoints = 50 * target.PlayerPawnsForStoryteller.Count(p =>
				{
					if (p.ParentHolder != null && p.ParentHolder is Building_CryptosleepCasket) //Presumably no heartbeats in a cryptosleep casket.
						return false;
					return true;
				}); //50 points per heartbeat.
			else if (rollingForRaid || rollingForNeutralGroup)
			{
				RebeccaLog("Rebecca is targeting \"" + target + "\" with a raid or neutral group.");
				//If the target's a map, get our MapComp and use the lerped wealth rather than the raw one, otherwise use raw one.
				float wealth = wealthForTarget(target);
				RebeccaLog("Rebecca is looking at your wealth, the world is aware of $" + wealth);
				//y=9742.433+(120.3303-9742.433)/(1+(x/1326793)^.9796777) per mycurvefit.com in symmetrical sigmoidal mode
				//for the points I plugged (partly based on vanilla curve):
				/*
				 *0 - 35
				 *14k - 300
				 *100k - 1k 
				 *200k - 1.2k 
				 *400k - 2.4k 
				 *700k - 3.6k
				 *1m - 4.2k 
				 */
				basePoints = 9742.433f+(120.3303f-9742.433f)/(float)(1f+Math.Pow((double)wealth/1326793f, .9796777f));
				if (rollingForNeutralGroup) //If it's a neutral group..
				{
					RebeccaLog("Rebecca is rolling for a neutral group. Before adjusting the points she has: " + basePoints + ".");
					basePoints = Math.Min(Rand.Range(100, basePoints), RebeccaSettings.VisitorMaxThreatPoints); //Let it drop as low as 100 randomly, cap it at a user setting that defaults at 3k.
					RebeccaLog("Rebecca adjusted the points, now she has: " + basePoints + ".");
				}
			}
			//if (!rollingForRaid)
			//	basePoints += 1000 * Mathf.Pow(Rand.Value, RebeccaSettings.HighThreatRarityExponent);
			else //Rebecca doesn't know what this is.
			{
				RebeccaLog("Rebecca doesn't know what a \"" + rollingForIncidentDef.defName + "\" is, so she's gonna use a curved RNG to determine points!");
				basePoints = 4200 * Mathf.Pow(Rand.Value, RebeccaSettings.HighThreatRarityExponent);
			}
			basePoints *= .6f; //Adjust to 60% of it since I balanced at Adventure Story difficulty to re-tune it for Strive to Survive.
			if (!rollingForNeutralGroup)
			{
				RebeccaLog("Rebecca's pre-multiplier points are: " + basePoints + ".");
				basePoints = Rand.Range(basePoints, basePoints * RebeccaSettings.ThreatPointsMultiplier); //Make it arbitrarily harder to represent how uncaring the universe is, but not necessarily always the multiplier, let it vary!
				RebeccaLog("Rebecca's post-multiplier points are: " + basePoints + ".");
			}
			float pointsAdjustedForDifficulty = basePoints * Find.Storyteller.difficulty.threatScale; //Apply the user's threat scale setting..
			RebeccaLog("Rebecca just calculated defaultThreatPointsNow " + 
				(rollingForRaid ? "(rolling for a raid) " : "") +
				(rollingForManhunters ? "(rolling for a manhunter pack) " : "") +
				(rollingForInfestation ? "(rolling for an infestation) " : "") +
				(rollingForNeutralGroup ? "(rolling for a neutral group) " : "") +
				"and got " + pointsAdjustedForDifficulty);
			return pointsAdjustedForDifficulty;
		}
	}
}