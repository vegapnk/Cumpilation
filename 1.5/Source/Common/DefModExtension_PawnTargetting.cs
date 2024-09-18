using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Common
{
    public abstract class DefModExtension_PawnTargetting : DefModExtension, IPawnTargetter
    {
        public bool allowMen = true;
        public bool allowWomen = true;
        public bool allowFutas = true;
        public bool allowAnimals = false;
        /// <summary>
        /// A list of hediffs that block spawning of this Hediff.
        /// If any is present, at any severity other than 0, `CanTargetPawn` will return false.
        /// </summary>
        public List<HediffDef> blockingHediffs = new List<HediffDef>();
        public List<TraitDef> blockingTraits = new List<TraitDef>();

        public bool onlyAdults = true;

        public bool IsValidPawn(Pawn pawn)
        {
            return
                PartUtility.IsValidPawn(pawn, allowMen: allowMen, allowWomen: allowWomen, allowFutas: allowFutas, allowAnimals: allowAnimals, blockingHediffs: blockingHediffs, blockingTraits: blockingTraits, onlyAdults:onlyAdults);
        }
    }
}
