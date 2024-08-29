using HarmonyLib;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cumpilation.Gathering
{
    [HarmonyPatch(typeof(SexUtility), nameof(SexUtility.SatisfyPersonal))] // Actual on orgasm method
    public static class Patch_SatisfyPersonal_FillFluidGatherers
    {
        public static void Postfix(SexProps props)
        {
            GatheringUtility.FillFluidGatherers(props);
        }
    }
}
