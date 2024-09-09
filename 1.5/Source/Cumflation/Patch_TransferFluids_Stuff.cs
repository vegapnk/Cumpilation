using HarmonyLib;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cumpilation.Cumflation
{
    [HarmonyPatch(typeof(SexUtility), nameof(SexUtility.TransferFluids))]
    public static class Patch_TransferFluids_Stuff
    {
        public static void Postfix(SexProps props)
        {
            if (Settings.Settings.EnableStuffing) { 
                StuffingUtility.StuffPawn(props.pawn, props.partner, props);
            }
        }
    }
}
