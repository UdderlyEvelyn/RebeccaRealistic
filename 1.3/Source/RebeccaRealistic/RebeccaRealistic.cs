using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;
using HarmonyLib;

namespace RR
{
    public class RebeccaRealistic : Mod
    {
        public RebeccaSettings Settings;

        public RebeccaRealistic(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("UdderlyEvelyn.RebeccaRealistic");
            harmony.PatchAll();
            Settings = GetSettings<RebeccaSettings>();
        }

        public override string SettingsCategory()
        {
            return "Rebecca Realistic";
        }

        private string _delayedWealthEffectPerDayBuffer;
        private string _twoAtOnceThreatBigChanceBuffer;
        private string _baseBonusThreatBigChanceBuffer;
        private string _bonusThreatBigChancePerWealthChanceBuffer;
        private string _bonusThreatBigChancePerWealthThresholdBuffer;
        private string _bonusThreatBigMinimumSpacingTicksBuffer;
        private string _bonusThreatbigMaximumSpacingTicksBuffer;
        private string _visitorChanceBuffer;
        private string _visitorIsOrbitalBuffer;
        private string _visitorMinimumSpacingTicksBuffer;
        private string _visitorMaximumSpacingTicksBuffer;
        private string _visitorMaxThreatPointsBuffer;
        private string _threatPointsMultiplierBuffer;
        private string _mtbUnitBuffer;
        private string _highThreatRarityExponentBuffer;

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.TextFieldNumericLabeled<float>("Delayed Wealth Effect Per Day (%)", ref RebeccaSettings.DelayedWealthEffectPerDay, ref _delayedWealthEffectPerDayBuffer, 0f, 1f);
            listingStandard.TextFieldNumericLabeled<float>("Two At Once ThreatBig Chance", ref RebeccaSettings.TwoAtOnceThreatBigChance, ref _twoAtOnceThreatBigChanceBuffer, 0f, 1f);
            listingStandard.TextFieldNumericLabeled<float>("Base Bonus ThreatBig Chance", ref RebeccaSettings.BaseBonusThreatBigChance, ref _baseBonusThreatBigChanceBuffer, 0f, 1f);
            listingStandard.TextFieldNumericLabeled<float>("Extra Chance Per X Wealth", ref RebeccaSettings.BonusThreatBigChancePerWealthChance, ref _bonusThreatBigChancePerWealthChanceBuffer, 0f, 1f);
            listingStandard.TextFieldNumericLabeled<float>("X Wealth", ref RebeccaSettings.BonusThreatBigChancePerWealthThreshold, ref _bonusThreatBigChancePerWealthThresholdBuffer, 1f);
            listingStandard.TextFieldNumericLabeled<int>("Bonus ThreatBig Minimum Spacing Ticks", ref RebeccaSettings.BonusThreatBigMinimumSpacingTicks, ref _bonusThreatBigMinimumSpacingTicksBuffer, 0f);
            listingStandard.TextFieldNumericLabeled<int>("Bonus ThreatBig Maximum Spacing Ticks", ref RebeccaSettings.BonusThreatBigMaximumSpacingTicks, ref _bonusThreatbigMaximumSpacingTicksBuffer, 0f);
            listingStandard.TextFieldNumericLabeled<float>("Visitor Chance", ref RebeccaSettings.VisitorChance, ref _visitorChanceBuffer, 0f, 1f);
            listingStandard.TextFieldNumericLabeled<float>("Chance Visitor Is Orbital", ref RebeccaSettings.VisitorIsOrbitalChance, ref _visitorIsOrbitalBuffer, 0f, 1f);
            listingStandard.TextFieldNumericLabeled<float>("Threat Points Multiplier", ref RebeccaSettings.ThreatPointsMultiplier, ref _threatPointsMultiplierBuffer, 0f);
            listingStandard.TextFieldNumericLabeled<int>("Visitor Minimum Spacing Ticks", ref RebeccaSettings.VisitorMinimumSpacingTicks, ref _visitorMinimumSpacingTicksBuffer, 0f);
            listingStandard.TextFieldNumericLabeled<int>("Visitor Maximum Spacing Ticks", ref RebeccaSettings.VisitorMaximumSpacingTicks, ref _visitorMaximumSpacingTicksBuffer, 0f);
            listingStandard.TextFieldNumericLabeled<float>("Visitor Max Threat Points", ref RebeccaSettings.VisitorMaxThreatPoints, ref _visitorMaxThreatPointsBuffer);
            listingStandard.TextFieldNumericLabeled<float>("MTB Unit (Lower Means More Incidents)", ref RebeccaSettings.MTBUnit, ref _mtbUnitBuffer, 0f);
            listingStandard.TextFieldNumericLabeled<float>("High Threat Rarity Exponent (Higher Means Rarer)", ref RebeccaSettings.HighThreatRarityExponent, ref _highThreatRarityExponentBuffer, 2f);
            listingStandard.CheckboxLabeled("Enable Logging", ref RebeccaSettings.LoggingEnabled, "Turn on logging for Rebecca so you can read her mind in the debug log.");
            listingStandard.CheckboxLabeled("Enable Log Nothings", ref RebeccaSettings.LogNothingsEnabled, "Enable logging for when nothing happens (e.g. \"Rebecca has decided to send nothing..\").");
            listingStandard.CheckboxLabeled("Enable Incident Selection Logging", ref RebeccaSettings.IncidentSelectionLoggingEnabled, "Turn on logging for incident selection (e.g. \"Rebecca is considering sending[..]\".");
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
    }
}