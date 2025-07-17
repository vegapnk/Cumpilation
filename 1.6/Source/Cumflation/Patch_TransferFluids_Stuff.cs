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
            if (Settings.EnableStuffing)
            {
                //Devnote: This was necessary for figuring out who is reverse in what.
                //PrintDebugInfoForSexRoles(props);
                
                if (!props.isReceiver && !props.isRevese)
                {
                    // Case A: The pawn fucks the partner, and shoots.
                    StuffingUtility.StuffPawn(props.pawn, props.partner, props);
                }
                if (props.isReceiver && props.isRevese)
                {
                    // Case B: The pawn reverse-fucks the partner, and the partner shoots.
                    StuffingUtility.StuffPawn(props.pawn, props.partner, props);
                }
            }
        }


        ///DevNotes, targetting & Roles from Testing:
        /// Tim -> Anal Rape -> Joe             := Joe=(Receiver, Reverse) , Tim=(!Receiver,!Reverse)
        /// Tim -> Anal Riding (Reverse) -> Joe := Joe=(Receiver, Reverse) , Tim=(!Receiver,Reverse)
        /// Tim -> Anal Rape -> Eva             := Eva=(Receiver, Reverse) , Tim=(!Receiver,!Reverse)
        /// Tim -> Cunnilingus (Reverse) -> Eva := Eva=(Receiver, Reverse) , Tim=(!Receiver,Reverse)
        private static void PrintDebugInfoForSexRoles(SexProps props)
        {
            if (!props.isReceiver && !props.isRevese)
            {
                ModLog.Debug($"{props.pawn} had orgasm but is receiving in reverse");
                return;
            }
            else if (!props.isReceiver && props.isRevese)
            {
                ModLog.Debug($"{props.pawn} had orgasm, is not receiver and is reverse");
            }
            else if (props.isReceiver && props.isRevese)
            {
                ModLog.Debug($"{props.pawn} had orgasm, is receiver and is reverse");
            }
            else if (!props.isRevese && !props.isReceiver)
            {
                ModLog.Debug($"{props.pawn} had orgasm, is not receiver and not reverse");
            }
        }
    }

}
