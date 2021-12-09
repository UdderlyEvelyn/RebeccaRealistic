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
        public static float BaseBonusThreatBigChance = .1f;
        public static float BonusThreatBigChancePerWealthChance = .01f;
        public static float BonusThreatBigChancePerWealthThreshold = 10000f;
        //public static int BonusThreatBigMinimumSpacingTicks = 2500;
        //public static int BonusThreatBigMaximumSpacingTicks = 3750;
        public static IntRange BonusThreatBigSpacingTicks = new IntRange(3500, 3750);
        public static float VisitorChance = .15f;
        public static float VisitorIsOrbitalChance = .46f;
        //public static int VisitorMinimumSpacingTicks = 1250;
        //public static int VisitorMaximumSpacingTicks = 2500;
        public static IntRange VisitorSpacingTicks = new IntRange(1250, 2500);
        public static float ThreatPointsMultiplier = 3f;
        public static float MTBUnit = 40000f;
        public static bool LoggingEnabled = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref BaseBonusThreatBigChance, "BaseBonusThreatBigChance");
            Scribe_Values.Look(ref BonusThreatBigChancePerWealthChance, "BonusThreatBigChancePerWealthChance");
            Scribe_Values.Look(ref BonusThreatBigChancePerWealthThreshold, "BonusThreatBigChancePerWealthThreshold");
            //Scribe_Values.Look(ref BonusThreatBigMinimumSpacingTicks, "BonusThreatBigMinimumSpacingTicks");
            //Scribe_Values.Look(ref BonusThreatBigMaximumSpacingTicks, "BonusThreatBigMaximumSpacingTicks");
            Scribe_Values.Look(ref BonusThreatBigSpacingTicks, "BonusThreatBigSpacingTicks");
            Scribe_Values.Look(ref VisitorChance, "VisitorChance");
            Scribe_Values.Look(ref VisitorIsOrbitalChance, "VisitorIsOrbitalChance");
            //Scribe_Values.Look(ref VisitorMinimumSpacingTicks, "VisitorMinimumSpacingTicks");
            //Scribe_Values.Look(ref VisitorMaximumSpacingTicks, "VisitorMaximumSpacingTicks");
            Scribe_Values.Look(ref VisitorSpacingTicks, "VisitorSpacingTicks");
            Scribe_Values.Look(ref ThreatPointsMultiplier, "ThreatPointsMultiplier");
            Scribe_Values.Look(ref MTBUnit, "MTBUnit");
            Scribe_Values.Look(ref LoggingEnabled, "LoggingEnabled");
            base.ExposeData();
        }
    }
}