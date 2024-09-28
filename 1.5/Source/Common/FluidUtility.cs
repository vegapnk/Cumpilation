using Cumpilation.Cumflation;
using Cumpilation.Gathering;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Common
{
    public class FluidUtility
    {

        public static void ChangeFluidType(ISexPartHediff part, SexFluidDef newFluid)
        {
            var comp = part.GetPartComp();
            ModLog.Debug($"Changing {part}s fluid for {part.GetOwner()} from {comp.Fluid} to {newFluid}");
            comp.Fluid = newFluid;
        }

        public static List<ISexPartHediff> GetGenitalsWithFluids(Pawn pawn, bool filterForShootsOnOrgasm = false)
        {
            List<ISexPartHediff> results = new List<ISexPartHediff>();            
            foreach (var part in rjw.Genital_Helper.get_AllPartsHediffList(pawn))
            {
                var def = GetHediffDefSexPart(part);
                ISexPartHediff partCasted = (ISexPartHediff)part;
                if (def == null) continue;
                if (partCasted.GetPartComp().Fluid == null) continue;
                if (filterForShootsOnOrgasm && !def.produceFluidOnOrgasm) continue;
                //ModLog.Debug($"Found Genital {part} for {pawn} with Fluid {partCasted.GetPartComp().Fluid}");
                results.Add(partCasted);
            }
            return results;
        }

        public static HediffDef_SexPart? GetHediffDefSexPart(Hediff hediff)
        {
            if (hediff == null) return null;
            if (hediff.def is HediffDef_SexPart)
                return (HediffDef_SexPart)(hediff.def);
            return null;
        }

        public static bool IsSexWithFluidFlyingAround(SexProps props)
        {
            xxx.rjwSextype sextype = props.sexType;
            bool fluidFlyingAroundSexType =
                // Base: Fill Cumbuckets on Masturbation. Having no partner means it must be masturbation too
                sextype == xxx.rjwSextype.Masturbation || props.partner == null
                || sextype == xxx.rjwSextype.Boobjob || sextype == xxx.rjwSextype.Footjob 
                || sextype == xxx.rjwSextype.Handjob || sextype == xxx.rjwSextype.Scissoring;
            return fluidFlyingAroundSexType;
            //TODO: Return true if there is a heavy missmatch of fluid-amount and body-size, or on full cumflation etc. 
            //return false;
        }

        /// <summary>
        /// Tries to store a record of `FluidSource` in a given Hediff.
        /// In case the Hediff does not have a `HediffComp_SourceStorage` nothing will happen. 
        /// If there already exists a Storage Entry for this Pawn and this fluid, it will be increased by the incoming amount.
        /// </summary>
        /// <param name="cumflationHediff">The hediff to store a record in</param>
        /// <param name="origin">The pawn that originated the Fluid</param>
        /// <param name="fluid">The type of fluid</param>
        /// <param name="amount">The amount of fluid</param>
        public static void StoreFluidSource(Hediff cumflationHediff, Pawn origin, SexFluidDef fluid, float amount)
        {
            var storage_comp = cumflationHediff.TryGetComp<HediffComp_SourceStorage>();
            if (storage_comp == null) return;

            FluidSource source = new FluidSource() { pawn = origin, fluid = fluid, amount = amount };
            storage_comp.AddOrMerge(source);
        }


        /// <summary>
        /// Returns true if the pawn likes cumflation / cumstuffing for one or the other reason. 
        /// Important: This is a separate method intentionally, so it can be patched. 
        /// </summary>
        /// <param name="inflated">The pawn that maybe likes cumflation</param>
        /// <returns>True, if the pawn likes cumflation from traits, quirks or zoophile.</returns>
        public static bool LikesCumflation(Pawn inflated)
        {
            bool likesCumflation = inflated?.story?.traits?.HasTrait(DefOfs.Cumpilation_LikesCumflation) ?? false;
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


        public static IEnumerable<SexFluidDef> GetAllSexFluidDefs()
        {
            return DefDatabase<SexFluidDef>.AllDefs;
        }
    }
}
