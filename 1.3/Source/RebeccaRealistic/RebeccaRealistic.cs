using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;

namespace RR
{
    public class RebeccaRealistic : Mod
    {
        public RebeccaSettings Settings;

        public RebeccaRealistic(ModContentPack content) : base(content)
        {
            Settings = GetSettings<RebeccaSettings>();
        }
    }
}
