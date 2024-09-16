using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Common
{
    public class DefModExtension_PartTargetting : DefModExtension, IPartTargetter
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


        /// <summary>
        /// A list of hediffs that block spawning of this Hediff.
        /// If any is present, at any severity other than 0, `CanTargetPawn` will return false.
        /// </summary>
        public List<HediffDef> blockingHediffs = new List<HediffDef>();

        public IEnumerable<ISexPartHediff> GetSexPartHediffs(Pawn pawn)
        {
            return PartUtility.FindFittingSexParts(pawn,
                targetPenis: this.targetPenis, targetVagina: this.targetVagina, targetBreast: this.targetBreast, targetAnus: this.targetAnus, targetOther: this.targetOther,
                allowMen: this.allowMen, allowWomen: this.allowWomen, allowAnimals: this.allowAnimals, allowFutas: this.allowFutas, 
                needsFluid: this.needsFluid, blockingHediffs: this.blockingHediffs);
        }

        public virtual bool CanTargetPawn (Pawn pawn) => GetSexPartHediffs(pawn).Count() > 0;

    }
}
