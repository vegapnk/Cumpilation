using Cumpilation.Common;
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
    public class ThoughtWorker_Cumslut_LookingForJuicyPenetrators : ThoughtWorker
    {

        const float DISLIKE_STRONGLY_THRESHOLD = 1.0f;
        const float DISLIKE_THRESHOLD = 9.0f; 
        const float LIKE_THRESHOLD = 30.0f;
        const float LIKE_STRONGLY_THRESHOLD = 100.0f;

        protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
        {
            // Return for trivial errors
            if (pawn == null || other == null || pawn == other)
                return (ThoughtState)false;
            // Check for position-existance
            if (pawn.Map == null || other.Map == null || !pawn.Spawned || !other.Spawned)
                return (ThoughtState)false;
            // Do nothing if pawn is carried 
            if (pawn.CarriedBy != null)
                return (ThoughtState)false;
            // Do nothing if Pawn is Baby or Child (#25)
            if (!pawn.ageTracker.Adult)
                return (ThoughtState)false;
            if (!other.ageTracker.Adult)
                return (ThoughtState)false;
            // Only check if they are spawned humans
            if (!pawn.Spawned || !other.Spawned)
                return (ThoughtState)false;
            if (!pawn.RaceProps.Humanlike)
                return (ThoughtState)false;
            if (!other.RaceProps.Humanlike)
                return (ThoughtState)false;

            // Pawns that have not "met" wont give each other Mali
            // Known-Each-Other is a key-word for Rimworld that shows they have had any interaction and stored each other in relations. 
            if (!RelationsUtility.PawnsKnowEachOther(pawn, other))
                return (ThoughtState)false;
            // If the pawn is not on Map (e.g. caravan), no mali 
            if (!MapUtility.PawnIsOnHomeMap(pawn))
                return (ThoughtState)false;


            // Do nothing if there is no size-blinded involved 
            if (!pawn.story.traits.HasTrait(DefOfs.Cumpilation_LikesCumflation))
            {
                return (ThoughtState)false;
            }

            var relevant_parts = Genital_Helper.get_AllPartsHediffList(other)
                .Where(x => x is ISexPartHediff)
                .Cast<ISexPartHediff>()
                .Where(sP => sP.GetPartComp().Fluid != null)
                .Where(sP => sP.GetPartComp().Def.genitalTags.Contains(GenitalTag.CanPenetrate) || sP.GetPartComp().Fluid.tags.Contains("CanCumflate"))
                .OrderByDescending(sP => sP.GetPartComp().FluidAmount)
                .ToList();

            foreach (ISexPartHediff part in relevant_parts)
            {
                var comp = part.GetPartComp();

                if (comp.FluidAmount >= LIKE_STRONGLY_THRESHOLD)
                {
                    return ThoughtState.ActiveAtStage(3);
                }
                else if (comp.FluidAmount >= LIKE_THRESHOLD)
                {   
                    return ThoughtState.ActiveAtStage(2);
                }
                else if (comp.FluidAmount <= DISLIKE_STRONGLY_THRESHOLD)
                {
                    return ThoughtState.ActiveAtStage(0);
                }
                else if (comp.FluidAmount <= DISLIKE_THRESHOLD)
                {
                    return ThoughtState.ActiveAtStage(1);
                }
            }

            return (ThoughtState)false;
        }
    }   
}
