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
    public class RebeccaRealistic : Mod
    {
        public RebeccaSettings Settings;

        public RebeccaRealistic(ModContentPack content) : base(content)
        {
            Settings = GetSettings<RebeccaSettings>();
        }
        public override string SettingsCategory()
        {
            return "Rebecca Realistic";
        }

        private string _baseBonusThreatBigChanceBuffer;
        private string _bonusThreatBigChancePerWealthChanceBuffer;
        private string _bonusThreatBigChancePerWealthThresholdBuffer;
        private string _visitorChanceBuffer;
        private string _visitorIsOrbitalBuffer;
        private string _threatPointsMultiplierBuffer;
        private string _mtbUnitBuffer;

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.TextFieldNumericLabeled<float>("Base Bonus ThreatBig Chance", ref RebeccaSettings.BaseBonusThreatBigChance, ref _baseBonusThreatBigChanceBuffer, 0f, 1f);
            listingStandard.TextFieldNumericLabeled<float>("Extra Chance Per X Wealth", ref RebeccaSettings.BonusThreatBigChancePerWealthChance, ref _bonusThreatBigChancePerWealthChanceBuffer, 0f, 1f);
            listingStandard.TextFieldNumericLabeled<float>("X Wealth", ref RebeccaSettings.BonusThreatBigChancePerWealthThreshold, ref _bonusThreatBigChancePerWealthThresholdBuffer, 1f);
            listingStandard.TextFieldNumericLabeled<float>("Visitor Chance", ref RebeccaSettings.VisitorChance, ref _visitorChanceBuffer, 0f, 1f);
            listingStandard.TextFieldNumericLabeled<float>("Chance Visitor Is Orbital", ref RebeccaSettings.VisitorIsOrbitalChance, ref _visitorIsOrbitalBuffer, 0f, 1f);
            listingStandard.TextFieldNumericLabeled<float>("Threat Points Multiplier", ref RebeccaSettings.ThreatPointsMultiplier, ref _threatPointsMultiplierBuffer, 0f);
            listingStandard.TextFieldNumericLabeled<float>("MTB Unit (Lower Means More Incidents)", ref RebeccaSettings.MTBUnit, ref _mtbUnitBuffer, 0f);
            listingStandard.CheckboxLabeled("Enable Logging", ref RebeccaSettings.LoggingEnabled, "Turn on logging for Rebecca so you can read her mind in the debug log.");
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
    }
}