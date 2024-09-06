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
    public class ReactionUtility
    {
        public static bool PawnHasFittingTrait(Pawn pawn, IEnumerable<TraitDef> traits)
        {
            foreach (TraitDef trait in traits)
                if (pawn.story.traits.allTraits.Any(t => t.def == trait))
                    return true;
            return false;
        }

        public static bool PawnHasFittingQuirk(Pawn pawn, IEnumerable<string> quirks)
        {
            foreach (string quirk in quirks)
                if (xxx.has_quirk(pawn, quirk))
                    return true;

            return false;
        }

        public static Fluid_Record_Mapping LookupFluid(SexFluidDef fluid)
        {
            return DefDatabase<Fluid_Record_Mapping>.AllDefs.FirstOrFallback(f => f.fluid == fluid);
        }


        public static void PrintFluidRecordInfo()
        {
            IEnumerable<Fluid_Record_Mapping> defs = DefDatabase<Fluid_Record_Mapping>.AllDefs;

            ModLog.Debug($"Found {defs.Count()} Fluid_Record_Mapping");
        }
    }
}
