using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;
using Verse.Noise;
using RimWorld.QuestGen;
using rjw;
using Cumpilation.Cumflation;

namespace Cumpilation.Leaking
{
    public class Comp_ApparelSealCum : ThingComp
    {
        private CompProperties_ApparelSealCum Props => (CompProperties_ApparelSealCum)props;


        public override void Notify_Equipped(Pawn pawn)
        {
            pawn.health.AddHediff(DefOfs.Cumpilation_Sealed);
        }
        public override void Notify_Unequipped(Pawn pawn)
        {
            pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(DefOfs.Cumpilation_Sealed));
        }
    }
}