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
                    StoreFluidSource(cumflationHediff,inflator,genital.GetPartComp().Fluid,genital.GetPartComp().FluidAmount);

                    if (cumflationHediff.Severity > 1.01) 
                        TryQueueOverflowingCumflation(inflated);
                }
            }
        }

        public static void StuffPawn(Pawn inflator, Pawn inflated, SexProps props)
        {
            if (inflated == null || inflator == null || props == null) return;
            if (props.usedCondom) return;
            if (!IsSexTypeThatCanCumstuff(props)) return;

            //TODO: can we add a check/filter for the involved genitals? 
            var inflatorGenitals =
                FluidUtility.GetGenitalsWithFluids(inflator)
                .Where(genital => CanStuff(genital));

            // Due to logic, we have to go over one genital after the other. 
            foreach (ISexPartHediff genital in inflatorGenitals)
            {
                ModLog.Debug($"Stuffing {inflated} with {inflator}s {genital}, adding {genital.GetPartComp().Fluid}");
                float necessaryAmount = FluidAmountRequiredToStuffPawn(inflated, genital.GetPartComp().Fluid);
                float incomingSeverity = DetermineStuffingSeverity(inflated, genital.GetPartComp().FluidAmount, genital.GetPartComp().Fluid);

                int existingStuffingHediffs = GetAllSharedSeverityHediffsInPawn(inflated).Count();
                float accumulatedExistingSeverity = SumUpAllStuffingSeverities(inflated);
                float overflow = accumulatedExistingSeverity + incomingSeverity - 1;

                ModLog.Debug($"Adding {incomingSeverity} to {accumulatedExistingSeverity} in {inflated}, doing an overflow of {overflow} on {existingStuffingHediffs} existing stuffing-hediffs.");
                if (incomingSeverity > 0)
                {
                    Hediff stuffedHediff = GetOrCreateStuffHediff(inflated, genital.GetPartComp().Fluid);
                    stuffedHediff.Severity += Math.Min(1.0f,incomingSeverity);
                    ModLog.Debug($"Added or increased severity of {stuffedHediff} to Severity {stuffedHediff.Severity}");

                    if (overflow > 0) { 
                        foreach (Hediff sharedSeverityHediff in GetAllSharedSeverityHediffsInPawn(inflated))
                        {
                            if (sharedSeverityHediff.def != stuffedHediff.def && existingStuffingHediffs > 0) {
                                float reduction = overflow / (float)(existingStuffingHediffs);
                                ModLog.Debug($"Due to overflow of {overflow} in {existingStuffingHediffs-1} Stuff-Hediffs in {inflated}, reducing all old ones by {reduction}");
                                sharedSeverityHediff.Severity -= reduction;
                            }
                        }
                    }
                    StoreFluidSource(stuffedHediff, inflator, genital.GetPartComp().Fluid, genital.GetPartComp().FluidAmount);

                    // TODO: add thoughts ... 
                    // TODO: something fancier for the Overflow?
                }
            }
        }

        public static IEnumerable<Hediff> GetAllSharedSeverityHediffsInPawn(Pawn pawn)
        {
            return pawn.health
                .hediffSet.hediffs.Where(
                hediff => hediff.TryGetComp<HediffComp_SharedSeverityPerDay>() != null);
        }

        public static float SumUpAllStuffingSeverities(Pawn pawn) => GetAllSharedSeverityHediffsInPawn(pawn).Sum(hediff => hediff.Severity);

        public static void TryQueueOverflowingCumflation(Pawn inflated)
        {
            var overflowingJob = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("OverflowingCumflation"), inflated);
            inflated.jobs.jobQueue.EnqueueFirst(overflowingJob);

        }

        public static void StoreFluidSource(Hediff cumflationHediff, Pawn origin, SexFluidDef fluid, float amount)
        {
            var storage_comp = cumflationHediff.TryGetComp<HediffComp_SourceStorage>();
            if (storage_comp == null) return;

            FluidSource source = new FluidSource() { pawn = origin, fluid = fluid, amount = amount };
            storage_comp.AddOrMerge(source);
        }

        public static bool IsSexTypeThatCanCumflate(SexProps props) {
            return props.sexType == xxx.rjwSextype.Vaginal 
                || props.sexType == xxx.rjwSextype.DoublePenetration 
                || props.sexType == xxx.rjwSextype.Scissoring;
        }


        public static bool IsSexTypeThatCanCumstuff(SexProps props)
        {
            return props.sexType == xxx.rjwSextype.Anal
                || props.sexType == xxx.rjwSextype.DoublePenetration
                || props.sexType == xxx.rjwSextype.Oral
                || props.sexType == xxx.rjwSextype.Cunnilingus
                || props.sexType == xxx.rjwSextype.Fellatio
                || props.sexType == xxx.rjwSextype.Sixtynine;
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


        /// <summary>
        /// Gets (or creates) a Stuffing Hediff for the `inflated`-pawn, 
        /// located in the Genitals BodyPart. 
        /// The severity is set later in case of a new creation.
        /// </summary>
        /// <param name="inflated"></param>
        /// <returns>A Stuffed Hediff of the inflated pawn.</returns>
        public static Hediff GetOrCreateStuffHediff(Pawn inflated, SexFluidDef fluid)
        {
            BodyPartRecord bodyPartRecord = Genital_Helper.get_stomachBPR(inflated);
            StuffingDef stuffDef = DefDatabase<StuffingDef>.AllDefs.FirstOrDefault(stuffingDef => stuffingDef.fluid == fluid);
            if (stuffDef == null)
            {
                ModLog.Warning($"Tried to stuff {inflated} with {fluid}, but did not find a StuffingDef for it");
                return null;
            }

            Hediff stuffingHediff = inflated.health.hediffSet.GetFirstHediffOfDef(stuffDef.resultingStuffedHediff);
            if (stuffingHediff == null)
            {
                stuffingHediff = HediffMaker.MakeHediff(stuffDef.resultingStuffedHediff, inflated, bodyPartRecord);
                stuffingHediff.Severity = 0;
                inflated.health.AddHediff(stuffingHediff, bodyPartRecord);
            }
            return stuffingHediff;
        }

        /// <summary>
        /// Returns true if the pawn likes cumflation for one or the other reason. 
        /// Important: This is a separate method intentionally, so it can be patched. 
        /// </summary>
        /// <param name="inflated">The pawn that maybe likes cumflation</param>
        /// <returns>True, if the pawn likes cumflation from traits, quirks or zoophile.</returns>
        public static bool LikesCumflation(Pawn inflated)
        {
            bool likesCumflation = inflated?.story?.traits?.HasTrait(DefOfs.LikesCumflation) ?? false;
            if (likesCumflation)
            {
                return likesCumflation;
            }

            string pawn_quirks = CompRJW.Comp(inflated).quirks.ToString();
            if (pawn_quirks.Contains("Impregnation fetish") ||
                pawn_quirks.Contains("Teratophile") ||
                pawn_quirks.Contains("Incubator") ||
                pawn_quirks.Contains("Breeder") ||
                pawn_quirks.Contains("Messy") ||
                xxx.is_zoophile(inflated))
            {
                return true;
            }
            return false;
        }

        public static void GiveCumflationThoughts(Pawn inflated)
        {
            if (LikesCumflation(inflated))
                inflated?.needs?.mood?.thoughts?.memories?.TryGainMemory(DefOfs.GotOverCumflatedEnjoyed);
            else
                inflated?.needs?.mood?.thoughts?.memories?.TryGainMemory(DefOfs.GotOverCumflated);
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

        public static float DetermineStuffingSeverity(Pawn pawn, float fluidAmount, SexFluidDef def)
        {
            if (pawn == null || fluidAmount <= 0) return 0;

            float requiredAmount = FluidAmountRequiredToStuffPawn(pawn, def);

            if (requiredAmount <= 0)
            {
                ModLog.Warning($"Received a impossible Value for Required-Cumstuffing-Amount - 0.0 for stuffing {pawn} with {def}");
                return 0;
            }

            return fluidAmount / requiredAmount;
        }

        public static float FluidAmountRequiredToStuffPawn(Pawn pawn, SexFluidDef fluid)
        {
            StuffingDef sDef = DefDatabase<StuffingDef>.AllDefs.FirstOrDefault(stuffingDef => stuffingDef.fluid == fluid);
            if (pawn != null)
                return sDef.amountOfFluidForFullStuffing * pawn.BodySize;

            return sDef.amountOfFluidForFullStuffing;
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
                return 100.0f * pawn.BodySize;
            return 100.0f;
        }

        public static bool CanStuff(ISexPartHediff part)
        {
            return part.Def.genitalTags.Contains(GenitalTag.CanPenetrate) &&  CanStuff(part.GetPartComp().Fluid);
        }
        public static bool CanStuff(SexFluidDef def) =>  DefDatabase<StuffingDef>.AllDefs.Any(stuffingDef => stuffingDef.fluid == def);
        

        public static void PrintCumflatableInfo()
        {
            IEnumerable<SexFluidDef> defs = DefDatabase<SexFluidDef>.AllDefs;

            ModLog.Debug($"Found {defs.Count()} FluidGatheringDefs, of which {defs.Where(f => CanCumflate(f)).Count()} can cause cumflation.");
        }

        /// Math Example 1 for Cumstuffing: 
        /// Pawn has a 0.6 Stuffing for `Cum`, then a gentle insect comes and (would) stuff the Pawn with 1.2 `InsectSpunk`. 
        /// This results in a total stuffing of 1.8, and means 0.8 are overflow, as there can be atmost 1.0 stuffing. 
        /// The 0.8 overflow first press out all `Cum`, and then *vanish*. 
        /// The Pawn will have 1.0 `InsectSpunk-Stuffing` after the Process. 
        /// 
        /// Math Example 2 for Cumstuffing: 
        /// Pawn has 0.4 stuffing for `ChocciCum` and 0.4 stuffing for `InsectSpunk`, a total of 0.8 Stuffed. 
        /// Mr. Hose comes and wants to add another 0.4 `Cum` to the stuffing mix. 
        /// This results `0.4+0.4+0.4 = 1.2` Stuffed, and 0.2 Overflow. 
        /// The overflow is evenly substracted from all pre-existing stuffings. 
        /// The pawn will have 0.3 `CocchiCum`, 0.3 `InsectSpunk` and 0.4 `Cum` as his stuffing mix. 
    }
}
