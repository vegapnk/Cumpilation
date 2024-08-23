using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation
{
    [StaticConstructorOnStartup]
    static internal class HarmonyInit
    {
        static HarmonyInit()
        {
            Harmony harmony = new Harmony("vegapnk.cumpilation");

            harmony.PatchAll();
        }
    }
}
