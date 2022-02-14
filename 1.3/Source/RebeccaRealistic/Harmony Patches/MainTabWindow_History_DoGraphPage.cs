using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection.Emit;

namespace RR
{
    [HarmonyPatch(typeof(HistoryAutoRecorderGroup), "DrawGraph"), HarmonyDebug]
    public class HistoryAutoRecorderGroup_DrawGraph
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions, ILGenerator generator)
        {
            Label continueLabel = generator.DefineLabel();
            bool seenCorrectLdLoc2 = false;
            var instructions = codeInstructions.ToList();
            for (int i = 0; i < instructions.Count; i++)
            {
                var instruction = instructions[i];
                if (!seenCorrectLdLoc2 && instruction.opcode == OpCodes.Ldloc_2 && instructions[i-1].opcode == OpCodes.Callvirt)
                {
                    seenCorrectLdLoc2 = true;
                    instruction.labels.Add(continueLabel);
                    yield return instruction;
                }
                else if (i > 0 && instructions[i-1].opcode == OpCodes.Stloc_3)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    yield return CodeInstruction.Call("RR.HistoryAutoRecorderGroup_DrawGraph:shouldSkipGraph");
                    yield return new CodeInstruction(OpCodes.Brtrue_S, continueLabel);
                    yield return instruction;
                }
                else
                    yield return instruction;
            }
        }
        internal static bool shouldSkipGraph(HistoryAutoRecorder har)
        {
            return har.def.defName == "Wealth_Considered" && Find.Storyteller.def.defName != "Rebecca";
        }
    }
}