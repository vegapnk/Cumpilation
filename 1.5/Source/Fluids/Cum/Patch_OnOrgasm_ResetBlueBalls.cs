using HarmonyLib;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Fluids.Cum
{

    [HarmonyPatch(typeof(SexUtility), nameof(SexUtility.SatisfyPersonal))] // Actual on orgasm method
    public static class Patch_OnOrgasm_ResetBlueBalls
    {
        public static void Postfix(SexProps props) {
            if (props == null || props.pawn == null) return;
            if (props.orgasms == 0) return;

            if (props.isRevese && props.partner != null)
            {
                RemoveBlueBallsHediff(props.partner);
            } else if (!props.pawn.IsAnimal() && !props.pawn.Dead)
            {
                RemoveBlueBallsHediff(props.pawn);
            }
        }

        private static void RemoveBlueBallsHediff(Pawn pawn)
        {
            if ( !pawn.health.hediffSet.HasHediff(DefOfs.Cumpilation_BlueBalls))
            {
                return;
            }
            Hediff blueBallsHediff = CumHelper.GetOrCreateBlueBallsHediff(pawn);
            //ModLog.Debug($"Trying to remove {blueBallsHediff.def} with severity {blueBallsHediff.Severity} from {pawn}");
            pawn.health.RemoveHediff(blueBallsHediff);
        }
    }
}
