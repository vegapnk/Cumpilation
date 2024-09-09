using Cumpilation.Common;
using Cumpilation.Gathering;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static HarmonyLib.Code;

namespace Cumpilation.Cumflation
{
    public class CumflationUtility
    {
        /// <summary>
        /// Manages Cumflation by adding and increasing the relevant Hediff, checked for each Genital.
        /// 
        /// Also does the check IF a pawn should be cumflated.
        /// </summary>
        /// <param name="inflator">Donor of the fluid</param>
        /// <param name="inflated">Receiver of the fluid</param>
        /// <param name="props">Sexprops, always useful.</param>
        public static void CumflatePawn(Pawn inflator, Pawn inflated, SexProps props)
        {
            if (inflated == null || inflator == null || props == null) return;
            if (props.usedCondom) return;
            if (!IsSexTypeThatCanCumflate(props)) return;

            if (!CanBeCumflated(inflated)) return;

            //TODO: can we add a check/filter for the involved genitals? 
            var inflatorGenitals =
                FluidUtility.GetGenitalsWithFluids(inflator)
                .Where(genital => CanCumflate(genital));

            foreach(ISexPartHediff genital in inflatorGenitals)
            {
                float necessaryAmount = FluidAmountRequiredToCumflatePawn(inflated, genital.GetPartComp().Fluid);
                float resultingSeverity = DetermineCumflationSeverity(inflated, genital.GetPartComp().FluidAmount, genital.GetPartComp().Fluid);
                if (resultingSeverity > 0)
                {
                    Hediff cumflationHediff = GetOrCreateCumflationHediff(inflated);
                    cumflationHediff.Severity += resultingSeverity;
                    // Only give thoughts on more serious cumflations.
                    if (cumflationHediff.Severity >= 0.6)
                        GiveCumflationThoughts(inflated);
                    FluidUtility.StoreFluidSource(cumflationHediff,inflator,genital.GetPartComp().Fluid,genital.GetPartComp().FluidAmount);

                    if (cumflationHediff.Severity > 1.01) 
                        TryQueueOverflowingCumflation(inflated);
                }
            }
        }

        /// <summary>
        /// If the pawn is "overflowing", i.E. the severity is over 1, 
        /// it will try to start a Job to spray cum around.
        /// The Job will run until the Severity is below 1. 
        /// </summary>
        /// <param name="inflated">The pawn that might gets the Job.</param>
        public static void TryQueueOverflowingCumflation(Pawn inflated)
        {
            var overflowingJob = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("OverflowingCumflation"), inflated);
            inflated.jobs.jobQueue.EnqueueFirst(overflowingJob);

        }

        public static bool IsSexTypeThatCanCumflate(SexProps props) {
            return props.sexType == xxx.rjwSextype.Vaginal 
                || props.sexType == xxx.rjwSextype.DoublePenetration 
                || props.sexType == xxx.rjwSextype.Scissoring;
        }

        /// <summary>
        /// Gets (or creates) a Cumflation Hediff for the `inflated`-pawn, 
        /// located in the Genitals BodyPart. 
        /// The severity is set later in case of a new creation.
        /// </summary>
        /// <param name="inflated"></param>
        /// <returns>A Cumflation Hediff of the inflated pawn.</returns>
        public static Hediff GetOrCreateCumflationHediff(Pawn inflated)
        {
            BodyPartRecord bodyPartRecord = Genital_Helper.get_genitalsBPR(inflated);

            Hediff cumflationHediff = inflated.health.hediffSet.GetFirstHediffOfDef(DefOfs.Cumflation);
            if (cumflationHediff == null)
            {
                cumflationHediff = HediffMaker.MakeHediff(DefOfs.Cumflation, inflated, bodyPartRecord);
                cumflationHediff.Severity = 0;
                inflated.health.AddHediff(cumflationHediff, bodyPartRecord);
            }
            return cumflationHediff;
        }


        public static void GiveCumflationThoughts(Pawn inflated)
        {
            if (FluidUtility.LikesCumflation(inflated))
                inflated?.needs?.mood?.thoughts?.memories?.TryGainMemory(DefOfs.GotOverCumflatedEnjoyed);
            else
                inflated?.needs?.mood?.thoughts?.memories?.TryGainMemory(DefOfs.GotOverCumflated);
        }

        /// <summary>
        /// Modding - Hook. 
        /// Default: All Pawns can be cumflated. Animals cannot be cumflated.
        /// </summary>
        /// <param name="pawn">The pawn that might be cumflated.</param>
        /// <returns>True, unless patched.</returns>
        public static bool CanBeCumflated(Pawn pawn)
        {
            if (pawn == null) return false;
            if (pawn.IsAnimal()) return false;
            return true;
        }

        public static bool CanCumflate(ISexPartHediff part) => CanCumflate(part.GetPartComp().Fluid);

        public static bool CanCumflate(SexFluidDef def)
        {
            return def.tags.Select(tag => tag.ToLower()).Any(tag => tag == "cancumflate");
        }

        /// <summary>
        /// Calculates the resulting Cumflation-Severity relative to the fluid-amount and pawns body size. 
        /// Also used to determine "overflow". 
        /// 
        /// This method is also used to determine "cumstuffed" despite the name. 
        /// </summary>
        /// <param name="pawn">The pawn that might be cumflated</param>
        /// <param name="fluidAmount">The amount of fluid coming in.</param>
        /// <param name="def">FluidDef, optional for patching.</param>
        /// <returns>The severity of the resulting cumflation</returns>
        public static float DetermineCumflationSeverity(Pawn pawn, float fluidAmount, SexFluidDef def = null)
        {
            if (pawn == null || fluidAmount <= 0) return 0;

            float requiredAmount = FluidAmountRequiredToCumflatePawn(pawn, def);

            if (requiredAmount <= 0)
            {
                ModLog.Warning($"Received a impossible Value for Required-Cumflation-Amount - 0.0 for cumflating {pawn} with {def}");
                return 0;
            }

            return fluidAmount / requiredAmount;
        }


        /// <summary>
        /// We assume a simple relation: 100 fluid per 1 Bodysize. 
        /// The returning value will determine how much of the fluid is necessary for 1.0 cumflation severity.
        /// 
        /// This might be altered with Patches etc. or extra behaviour, which is why this is a separate method despite very simple logic. 
        /// </summary>
        /// <param name="pawn">The pawn for which </param>
        /// <param name="def">Optional for patches: For which fluid to check.</param>
        /// <returns>A number how much fluid amount is necessary for a "full" cumflation, i.E. a cumflation result resulting in 1.0 cumflation severity. </returns>
        public static float FluidAmountRequiredToCumflatePawn(Pawn pawn, SexFluidDef fluid = null)
        {
            if (pawn != null) 
                return 100.0f * pawn.BodySize * Settings.Settings.GlobalCumflationModifier;
            return 100.0f * Settings.Settings.GlobalCumflationModifier;
        }

        public static void PrintCumflatableInfo()
        {
            IEnumerable<SexFluidDef> defs = DefDatabase<SexFluidDef>.AllDefs;

            ModLog.Debug($"Found {defs.Count()} FluidGatheringDefs, of which {defs.Where(f => CanCumflate(f)).Count()} can cause cumflation.");
        }

    }
}
