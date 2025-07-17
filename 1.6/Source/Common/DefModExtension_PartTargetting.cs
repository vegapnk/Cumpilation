using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Common
{
    public class DefModExtension_PartTargetting : DefModExtension_PawnTargetting, IPartTargetter
    {
        public bool targetPenis = false;
        public bool targetVagina = false;
        public bool targetBreast = false;
        public bool targetAnus = false;
        public bool targetOther = false;

        public bool needsFluid = true;


        public IEnumerable<ISexPartHediff> GetSexPartHediffs(Pawn pawn)
        {
            if (IsValidPawn(pawn))
            {
                return PartUtility.FindFittingSexParts(pawn,
                    targetPenis: this.targetPenis, targetVagina: this.targetVagina, targetBreast: this.targetBreast, targetAnus: this.targetAnus, targetOther: this.targetOther,
                    needsFluid: this.needsFluid);
            }
            else 
                return new List<ISexPartHediff>();
        }

    }
}
