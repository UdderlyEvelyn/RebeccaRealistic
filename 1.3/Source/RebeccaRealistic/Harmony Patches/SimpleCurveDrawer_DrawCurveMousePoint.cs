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
    [HarmonyPatch(typeof(SimpleCurveDrawer), "DrawCurveMousePoint")]
    public class SimpleCurveDrawer_DrawCurveMousePoint
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            foreach (var instruction in codeInstructions)
                if (instruction.opcode == OpCodes.Ldc_R4 && instruction.OperandIs(120f))
                    yield return new CodeInstruction(OpCodes.Ldc_R4, 140f);
                else
                    yield return instruction;
        }
    }
}