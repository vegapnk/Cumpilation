using Cumpilation.Common;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Cumflation
{
    public class StuffingUtility
    {

        /// <summary>
        /// Manages Stuffing by adding and increasing the relevant Hediff, checked for each Genital.
        /// If the pawn is already "fully stuffed", old liquids get pushed out by reducing their severity.
        /// See the bottom of `StuffingUtility` for examples on the Behaviour.
        /// 
        /// Also does the check IF a pawn should be stuffed.
        /// </summary>
        /// <param name="inflator">Donor of the fluid</param>
        /// <param name="inflated">Receiver of the fluid</param>
        /// <param name="props">Sexprops, always useful.</param>
        public static void StuffPawn(Pawn inflator, Pawn inflated, SexProps props)
        {
            if (inflated == null || inflator == null || props == null) return;
            if (props.usedCondom) return;
            if (!IsSexTypeThatCanCumstuff(props)) return;

            if (!CanBeStuffed(inflated)) return;

            //TODO: can we add a check/filter for the involved genitals? 
            var inflatorGenitals =
                FluidUtility.GetGenitalsWithFluids(inflator)
                .Where(genital => CanStuff(genital));

            // Due to logic, we have to go over one genital after the other. 
            foreach (ISexPartHediff genital in inflatorGenitals)
            {
                if (inflated.IsAnimal() && !CanStuffAnimals(genital.GetPartComp().Fluid)) continue; 

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
                    stuffedHediff.Severity += Math.Min(1.0f, incomingSeverity);
                    ModLog.Debug($"Added or increased severity of {stuffedHediff} to Severity {stuffedHediff.Severity}");

                    if (overflow > 0)
                    {
                        foreach (Hediff sharedSeverityHediff in GetAllSharedSeverityHediffsInPawn(inflated))
                        {
                            if (sharedSeverityHediff.def != stuffedHediff.def && existingStuffingHediffs > 0)
                            {
                                float reduction = overflow / (float)(existingStuffingHediffs);
                                ModLog.Debug($"Due to overflow of {overflow} in {existingStuffingHediffs - 1} Stuff-Hediffs in {inflated}, reducing all old ones by {reduction}");
                                sharedSeverityHediff.Severity -= reduction;
                            }
                        }
                    }
                    FluidUtility.StoreFluidSource(stuffedHediff, inflator, genital.GetPartComp().Fluid, genital.GetPartComp().FluidAmount);

                    // TODO: add thoughts ... 
                    // TODO: something fancier for the Overflow?
                }
            }
        }

        /// <summary>
        /// Modding - Hook. 
        /// Default: All Pawns can be Stuffed.
        /// </summary>
        /// <param name="pawn">The pawn that might be stuffed.</param>
        /// <returns>True, unless patched.</returns>
        public static bool CanBeStuffed(Pawn pawn)
        {
            if (pawn == null) return false;
            return true;
        }

        public static bool CanStuffAnimals(SexFluidDef fluid)
        {
            var stuffDef = DefDatabase<StuffingDef>.AllDefs.FirstOrDefault(stuffingDef => stuffingDef.fluid == fluid);
            return stuffDef != null ? stuffDef.canStuffAnimals : false;
        }

        public static IEnumerable<Hediff> GetAllSharedSeverityHediffsInPawn(Pawn pawn)
        {
            return pawn.health
                .hediffSet.hediffs.Where(
                hediff => hediff.TryGetComp<HediffComp_SharedSeverityPerDay>() != null);
        }

        public static float SumUpAllStuffingSeverities(Pawn pawn) => GetAllSharedSeverityHediffsInPawn(pawn).Sum(hediff => hediff.Severity);


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
        /// Calculates the resulting severity for a given pawn.
        /// Will return a float bigger than 0.0, potentially bigger than 1.0;
        /// There is no limit at 1 because this method is also used for the overflow. 
        /// </summary>
        /// <param name="pawn">The pawn that will be checked for - bodysize is important.</param>
        /// <param name="fluidAmount">The amount of incoming fluid</param>
        /// <param name="def">The incoming fluid</param>
        /// <returns>The severity of a stuffing-hediff given bodysize and fluid-amount.</returns>
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
        public static bool CanStuff(ISexPartHediff part)
        {
            return part.Def.genitalTags.Contains(GenitalTag.CanPenetrate) && CanStuff(part.GetPartComp().Fluid);
        }
        public static bool CanStuff(SexFluidDef def) => DefDatabase<StuffingDef>.AllDefs.Any(stuffingDef => stuffingDef.fluid == def);


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
