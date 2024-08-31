using Cumpilation.Gathering;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Cumflation
{
    public class CumflationUtility
    {

        public static void PrintCumflatableInfo()
        {
            IEnumerable<SexFluidDef> defs = DefDatabase<SexFluidDef>.AllDefs;
            
            ModLog.Debug($"Found {defs.Count()} FluidGatheringDefs, of which {defs.Where(f => CanCumflate(f)).Count()} can cause cumflation.");
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

        public static void GiveOverinflationThoughts(Pawn inflated, xxx.rjwSextype sextype)
        {
            bool likesCumflation = LikesCumflation(inflated);

            switch (sextype)
            {
                /*
            case xxx.rjwSextype.Oral:
            case xxx.rjwSextype.Fellatio:
                if (likesCumflation)
                {
                    inflated?.needs?.mood?.thoughts?.memories?.TryGainMemory(DefOfs.GotOverCumstuffedEnjoyed);
                    return;
                }
                inflated?.needs?.mood?.thoughts?.memories?.TryGainMemory(DefOfs.GotOverCumstuffed);
                return;
                */
                case xxx.rjwSextype.Vaginal:
                case xxx.rjwSextype.Anal:
                case xxx.rjwSextype.DoublePenetration:
                    if (likesCumflation)
                    {
                        inflated?.needs?.mood?.thoughts?.memories?.TryGainMemory(DefOfs.GotOverCumflatedEnjoyed);
                        return;
                    }
                    inflated?.needs?.mood?.thoughts?.memories?.TryGainMemory(DefOfs.GotOverCumflated);
                    return;
                default:
                    return;
            }
        }

        /*
        public static void GiveCumflationThoughts(Pawn inflated)
        {
            if (!LikesCumflation(inflated))
            {
                return;
            }

            inflated?.needs?.mood?.thoughts?.memories?.TryGainMemory(DefOfs.GotInflatedKinky);
        }
        */
        public static bool CanCumflate(ISexPartHediff part) => CanCumflate(part.GetPartComp().Fluid);

        public static bool CanCumflate(SexFluidDef def)
        {
            return def.tags.Select(tag => tag.ToLower()).Any(tag => tag == "cancumflate");
        }
    }
}
