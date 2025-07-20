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
    /// <summary>
    /// 
    /// 
    /// Initially taken from Ameravashis Sexperience, extended to cover multiple fluids, optional quirks and traits that hardwire results.
    /// </summary>
    public class ThoughtDefExtension_StageFromConsumption : DefModExtension
    {
        public RecordDef recordDef;
        public List<float> minimumValueforStage = new List<float>();

        public List<TraitDef> traitsThatLike = new List<TraitDef>();
        public List<TraitDef> traitsThatDislike = new List<TraitDef>();
        public List<string> quirksThatLike = new List<string>();
        public List<string> quirksThatDislike = new List<string>();

        public int GetStageIndex(Pawn pawn)
        {
            float recordValue = pawn?.records?.GetValue(recordDef) ?? 0f;

            // We first check: If there are any traits, and no "counter-traits" that ruin it, we make things the worst or best respectively. 
            ModLog.Debug($"Checking for CumSlut trait on pawn named - {pawn.Name}.");
            if (ReactionUtility.PawnHasFittingTrait(pawn, traitsThatLike) && !ReactionUtility.PawnHasFittingTrait(pawn, traitsThatDislike))
                return LastStage();
            if (ReactionUtility.PawnHasFittingTrait(pawn, traitsThatDislike) && !ReactionUtility.PawnHasFittingTrait(pawn, traitsThatLike))
                return 0;

            // Then we do the same for Quirks. But Traits > Quirks
            // TODO
            // 1.6 quirks now in a seperate mod, notalways ran.
            /*
            if (ReactionUtility.PawnHasFittingQuirk(pawn, quirksThatLike) && !ReactionUtility.PawnHasFittingQuirk(pawn, quirksThatDislike))
                return LastStage();
            if (ReactionUtility.PawnHasFittingQuirk(pawn, quirksThatDislike) && !ReactionUtility.PawnHasFittingQuirk(pawn, quirksThatLike))
                return 0;
            */

            // Otherwise, if there are draws or nothing else happening, we look up the thresholds of the record.
            for (int i = minimumValueforStage.Count - 1; i > 0; i--)
            {
                if (minimumValueforStage[i] < recordValue)
                {
                    return i;
                }
            }

            return 0;
        }

        //TODO: Add Helper Methods for these checks, to have something patchable. 

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
            {
                yield return error;
            }

            if (recordDef == null)
            {
                yield return "<recordDef> is null";
            }

            if (minimumValueforStage.NullOrEmpty())
            {
                yield return "<minimumValueforStage> should have an entry for every stage";
            }

            for (int i = 0; i < minimumValueforStage.Count - 1; i++)
            {
                if (minimumValueforStage[i] > minimumValueforStage[i + 1])
                {
                    yield return "Values in <minimumValueforStage> should be ordered from the lowest to the highest";
                }
            }
        }

        public int LastStage() => this.minimumValueforStage.Count() - 1;

    }
}
