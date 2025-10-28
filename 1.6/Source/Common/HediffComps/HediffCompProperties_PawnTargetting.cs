using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Common
{
    public class HediffCompProperties_PawnTargetting : HediffCompProperties, IPawnTargetter
    {
        public bool allowMen = true;
        public bool allowWomen = true;
        public bool allowFutas = true;
        public bool allowAnimals = Settings.EnableOscillationMechanicsForAnimals;
        //public bool allowAnimals = false;

        public List<HediffDef> blockingHediffs = new List<HediffDef>();
        public List<TraitDef> blockingTraits = new List<TraitDef>();

        public bool onlyAdults = true;

        public bool IsValidPawn(Pawn pawn)
        {
            return
                PartUtility.IsValidPawn(pawn, allowMen: allowMen, allowWomen: allowWomen, allowFutas: allowFutas, allowAnimals: allowAnimals, blockingHediffs: blockingHediffs, blockingTraits: blockingTraits, onlyAdults: onlyAdults);
        }
    }
}
