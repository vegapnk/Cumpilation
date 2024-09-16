using Cumpilation.Common;
using Cumpilation.Cumflation;
using HarmonyLib;
using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Reactions
{
    [HarmonyPatch(typeof(SexUtility), nameof(SexUtility.TransferFluids))]
    public static class Patch_TransferFluid_UpdateRecordAndGiveThoughts
    {
        public static void Postfix(SexProps props) {
            if (props == null || props.pawn == null || props.partner == null || props.partner.IsAnimal() || props.partner.IsColonyMech) return;
            Pawn pawn = props.pawn;
            Pawn partner = props.partner;

            if (props.usedCondom) return;
            if (!StuffingUtility.IsSexTypeThatCanCumstuff(props)) return;

            /// DevNote: See `Patch_TransferFluids_Stuff` at the Bottom for how to figure out the cases.
            if (props.isReceiver && !props.isRevese)
            {
                // Case A: The pawn fucks the partner, and shoots.
                return;
            }
            if (!props.isReceiver && props.isRevese)
            {
                // Case B: The pawn reverse-fucks the partner, and the partner shoots.
                return;
            }


            var parts = FluidUtility.GetGenitalsWithFluids(pawn)
                .Where(p => p.Def.genitalTags.Contains(GenitalTag.CanPenetrate));

            ModLog.Debug($"{partner} is receiver, {pawn} is source --- continuing to check {parts.Count()} of their parts");
            foreach (var part in parts) {
                var recordMapping = ReactionUtility.LookupFluid(part.GetPartComp().Fluid);
                if (recordMapping == null) continue;
                partner.records.Increment(recordMapping.numConsumedRecord);
                partner.records.AddTo(recordMapping.amountConsumedRecord, part.GetPartComp().FluidAmount);
                ModLog.Debug($"Bumping {partner}s records for {part.GetPartComp().Fluid} by {part.GetPartComp().FluidAmount}. New:#{partner.records.GetValue(recordMapping.numConsumedRecord)} dinings ({partner.records.GetValue(recordMapping.amountConsumedRecord)} ml total)");
                
                if (Settings.EnableProgressingConsumptionThoughts)
                {
                    ReactionUtility.TryGiveThought(recordMapping, partner);
                }
            }
        }
    }

}
