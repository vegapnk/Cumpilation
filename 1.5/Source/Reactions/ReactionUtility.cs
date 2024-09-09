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
            if (pawn == null || pawn.story == null || pawn.story.traits == null ||  traits == null) return false;

            foreach (TraitDef trait in traits)
                if (pawn.story.traits.allTraits.Any(t => t.def == trait))
                    return true;
            return false;
        }

        public static bool PawnHasFittingQuirk(Pawn pawn, IEnumerable<string> quirks)
        {
            if (pawn == null || quirks == null) return false;

            foreach (string quirk in quirks)
                if (xxx.has_quirk(pawn, quirk))
                    return true;

            return false;
        }

        public static Fluid_Record_Mapping LookupFluid(SexFluidDef fluid)
        {
            return DefDatabase<Fluid_Record_Mapping>.AllDefs.FirstOrFallback(f => f.fluid == fluid);
        }

        public static ThoughtDef LookupThought(RecordDef record) {
            return DefDatabase<ThoughtDef>.AllDefs
                .FirstOrFallback(p => { 
                    var ext = p.GetModExtension<ThoughtDefExtension_StageFromConsumption>(); 
                    if (ext == null) return false;
                    return ext.recordDef == record;    
                }, null);
        }

        public static void TryGiveThought(Fluid_Record_Mapping recordMapping, Pawn pawn)
        {
            ThoughtDef thoughtDef = LookupThought(recordMapping.numConsumedRecord);
            if (thoughtDef == null) return;

            ModLog.Debug($"Found {thoughtDef} for Record {recordMapping.numConsumedRecord}");
            pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(thoughtDef);
        }

        public static void PrintFluidRecordInfo()
        {
            IEnumerable<Fluid_Record_Mapping> defs = DefDatabase<Fluid_Record_Mapping>.AllDefs;

            var relevantThoughtDefs = DefDatabase<ThoughtDef>.AllDefs
                .Where(p => p.GetModExtension<ThoughtDefExtension_StageFromConsumption>() != null);

            ModLog.Debug($"Found {defs.Count()} Fluid_Record_Mapping, and {relevantThoughtDefs.Count()} Thoughts with a relevant ThoughtDefExtension");
        }

    }
}
