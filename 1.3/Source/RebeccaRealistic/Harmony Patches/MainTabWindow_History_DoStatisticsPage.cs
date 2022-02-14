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
    [HarmonyPatch(typeof(MainTabWindow_History), "DoStatisticsPage"), HarmonyDebug]
    internal class MainTabWindow_History_DoStatisticsPage
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions, ILGenerator generator)
        {
            Label nextPop = generator.DefineLabel();
            bool poppedOnce = false;
            bool seenLdstrF0 = false;
            int skipCounter = 4;
            foreach (var instruction in codeInstructions)
            {
                if (!seenLdstrF0 && instruction.opcode == OpCodes.Ldstr && instruction.OperandIs("F0"))
                {
                    seenLdstrF0 = true;
                    yield return instruction;
                    continue;
                }
                if (!poppedOnce && seenLdstrF0)
                {
                    if (skipCounter > 0)
                    {
                        skipCounter--;
                        yield return instruction;
                        continue;
                    }
                    //Insert our code..
                    //Conditional so it doesn't do it if it's not set to Rebecca..
                    yield return CodeInstruction.Call("RR.MainTabWindow_History_DoStatisticsPage:isRebeccaEnabled"); //Is it Rebecca? -> stack
                    yield return new CodeInstruction(OpCodes.Brfalse_S, nextPop); //If that bool is false, skip our patched-in code.
                    //Do our patch as normal..
                    yield return new CodeInstruction(OpCodes.Pop); //Yeet extra StringBuilder from stack.
                    yield return new CodeInstruction(OpCodes.Ldloc_0); //Get the normal one onto the stack (follows original method's pattern).
                    yield return CodeInstruction.Call("RR.MainTabWindow_History_DoStatisticsPage:getConsideredWealthStringForMap"); //Get our value, formatted to string as in the other bits of the method.
                    yield return CodeInstruction.Call("System.Text.StringBuilder:AppendLine", new Type[] { typeof(string) }); //Append to the StringBuilder.
                    //Back to our scheduled programming..
                    instruction.labels.Add(nextPop); //Add nextPop label to the line that would go here normally.
                    yield return instruction; //Don't forget the thing that triggered it to pop!
                    poppedOnce = true; //Don't let it pop again.
                }
                else
                    yield return instruction;
            }
        }

        internal static string getConsideredWealthStringForMap()
        {
            return "ThisMapColonyConsideredWealth".Translate() + ": " + ConsideredWealthCompCache.GetFor(Find.CurrentMap).ConsideredWealth.ToString("F0");
        }

        internal static bool isRebeccaEnabled()
        {
            return Find.Storyteller.def.defName == "Rebecca";
        }
    }
}