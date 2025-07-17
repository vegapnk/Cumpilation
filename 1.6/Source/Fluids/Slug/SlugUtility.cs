using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Fluids.Slug
{
    public class SlugUtility
    {
        public static Hediff GetOrCreateToxicBuildUpHediff(Pawn pawn)
        {
            if (pawn == null || pawn.health == null) return null;

            Hediff toxicBuildup = pawn.health.hediffSet.GetFirstHediffOfDef(RimWorld.HediffDefOf.ToxicBuildup);
            if (toxicBuildup == null)
            {
                toxicBuildup = HediffMaker.MakeHediff(RimWorld.HediffDefOf.ToxicBuildup, pawn);
                toxicBuildup.Severity = 0;
                pawn.health.AddHediff(toxicBuildup);
            }
            return toxicBuildup;
        }
    }
}
