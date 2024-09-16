using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static HarmonyLib.Code;

namespace Cumpilation.Common
{
    public abstract class HediffCompProperties_PartTargetting : HediffCompProperties, IPartTargetter
    {
        public bool allowMen = true;
        public bool allowWomen = true;
        public bool allowFutas = true;
        public bool allowAnimals = false;

        public bool targetPenis = false;
        public bool targetVagina = false;
        public bool targetBreast = false;
        public bool targetAnus = false;
        public bool targetOther = false;

        public bool needsFluid = true;

        public bool onlyFirst = false;

        public List<HediffDef> blockingHediffs = new List<HediffDef>();

        public IEnumerable<ISexPartHediff> GetSexPartHediffs(Pawn pawn)
        {
            return PartUtility.FindFittingSexParts(pawn, 
                targetPenis: this.targetPenis, targetVagina: this.targetVagina, targetBreast: this.targetBreast, targetAnus: this.targetAnus, targetOther: this.targetOther,
                allowMen: this.allowMen, allowWomen: this.allowWomen, allowAnimals: this.allowAnimals, allowFutas: this.allowFutas, onlyFirst: this.onlyFirst, needsFluid: this.needsFluid, blockingHediffs: this.blockingHediffs);
        }
    }
}
