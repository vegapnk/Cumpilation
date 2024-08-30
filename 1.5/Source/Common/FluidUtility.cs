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
            ;
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
                || sextype == xxx.rjwSextype.Boobjob || sextype == xxx.rjwSextype.Footjob || sextype == xxx.rjwSextype.Handjob;
            return fluidFlyingAroundSexType;
            //TODO: Return true if there is a heavy missmatch of fluid-amount and body-size, or on full cumflation etc. 
            //return false;
        }

        public static IEnumerable<SexFluidDef> GetAllSexFluidDefs()
        {
            return DefDatabase<SexFluidDef>.AllDefs;
        }
    }
}
