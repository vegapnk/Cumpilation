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
    public static class Patch_TransferFluids_Cumflate
    {
        public static void Postfix(SexProps props)
        {
            //TODO: Add Settings to Turn Off


            CumflationUtility.CumflatePawn(props.pawn, props.partner,props);
        }

    }

}
