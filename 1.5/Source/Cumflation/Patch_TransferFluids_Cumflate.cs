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
            if (Settings.EnableCumflation) {
                /// DevNote: See `Patch_TransferFluids_Stuff` at the Bottom for how to figure out the cases.
                if (!props.isReceiver && !props.isRevese)
                {
                    // Case A: The pawn fucks the partner, and shoots.
                    CumflationUtility.CumflatePawn(props.pawn, props.partner, props);
                }
                if (props.isReceiver && props.isRevese)
                {
                    // Case B: The pawn reverse-fucks the partner, and the partner shoots.
                    CumflationUtility.CumflatePawn(props.pawn, props.partner, props);
                }
            }
        }

    }

}
