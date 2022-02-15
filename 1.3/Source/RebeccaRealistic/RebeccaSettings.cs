using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace RR
{
    public class RebeccaSettings : ModSettings
    {
        public static float DelayedWealthEffectPerDay = .1f;
        public static float TwoAtOnceThreatBigChance = .3f;
        public static float BaseBonusThreatBigChance = .06f; //Used to be .1f for most of Rebecca's in-dev life, then .07f for public testing stuff, .06f for release.
        public static float BonusThreatBigChancePerWealthChance = .01f;
        public static float BonusThreatBigChancePerWealthThreshold = 10000f;
        public static int BonusThreatBigMinimumSpacingTicks = 10000; //Was 2500 through most of Rebecca's in-dev life.
        public static int BonusThreatBigMaximumSpacingTicks = 60000; //Was 3750 through most of Rebecca's in-dev life.
        public static float VisitorChance = .15f;
        public static float VisitorIsOrbitalChance = .46f;
        public static int VisitorMinimumSpacingTicks = 1250;
        public static int VisitorMaximumSpacingTicks = 60000; //Was 2500 through most of Rebecca's in-dev life.
        public static float VisitorMaxThreatPoints = 3000;
        public static float ThreatPointsMultiplier = 3f;
        public static float MTBUnit = 80000f; //Was 40000 through most of Rebecca's in-dev life.
        public static float HighThreatRarityExponent = 3.5f;
        public static bool LoggingEnabled = false;
        public static bool LogNothingsEnabled = false;
        public static bool IncidentSelectionLoggingEnabled = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref DelayedWealthEffectPerDay, "DelayedWealthEffectPerDay");
            Scribe_Values.Look(ref TwoAtOnceThreatBigChance, "TwoAtOnceThreatBigChance");
            Scribe_Values.Look(ref BaseBonusThreatBigChance, "BaseBonusThreatBigChance");
            Scribe_Values.Look(ref BonusThreatBigChancePerWealthChance, "BonusThreatBigChancePerWealthChance");
            Scribe_Values.Look(ref BonusThreatBigChancePerWealthThreshold, "BonusThreatBigChancePerWealthThreshold");
            Scribe_Values.Look(ref BonusThreatBigMinimumSpacingTicks, "BonusThreatBigMinimumSpacingTicks");
            Scribe_Values.Look(ref BonusThreatBigMaximumSpacingTicks, "BonusThreatBigMaximumSpacingTicks");
            Scribe_Values.Look(ref VisitorChance, "VisitorChance");
            Scribe_Values.Look(ref VisitorIsOrbitalChance, "VisitorIsOrbitalChance");
            Scribe_Values.Look(ref VisitorMinimumSpacingTicks, "VisitorMinimumSpacingTicks");
            Scribe_Values.Look(ref VisitorMaximumSpacingTicks, "VisitorMaximumSpacingTicks");
            Scribe_Values.Look(ref VisitorMaxThreatPoints, "VisitorMaxThreatPoints");
            Scribe_Values.Look(ref ThreatPointsMultiplier, "ThreatPointsMultiplier");
            Scribe_Values.Look(ref MTBUnit, "MTBUnit");
            Scribe_Values.Look(ref HighThreatRarityExponent, "HighThreatRarityExponent");
            Scribe_Values.Look(ref LoggingEnabled, "LoggingEnabled");
            Scribe_Values.Look(ref LogNothingsEnabled, "LogNothingsEnabled");
            Scribe_Values.Look(ref IncidentSelectionLoggingEnabled, "IncidentSelectionLoggingEnabled");

            base.ExposeData();
        }
    }
}