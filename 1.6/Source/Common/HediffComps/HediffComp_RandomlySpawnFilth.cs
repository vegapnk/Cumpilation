using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Common
{
    public class HediffComp_RandomlySpawnFilth : HediffComp
    {

        public HediffCompProperties_RandomlySpawnFilth Props => (HediffCompProperties_RandomlySpawnFilth)this.props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if (parent.pawn == null || parent.pawn.Map == null) return;
            if (!parent.pawn.IsHashIntervalTick(Props.ticksBetweenCheck)) return;

            if (parent.def.stages.Count > 0 || Props.onlyAtStagesHigherThan > 0)
                if (parent.CurStageIndex <= Props.onlyAtStagesHigherThan)
                    return;

            var rand = new Random();
            foreach (var sexPart in Props.GetSexPartHediffs(parent.pawn))
            {
                float chance = Props.chanceToSpawn * (Props.chanceBasedOnSeverity ? parent.Severity : 1.0f);

                if (rand.NextDouble() < chance 
                    && sexPart.GetPartComp().Fluid != null 
                    && sexPart.GetPartComp().Fluid.filth != null)
                {
                    FilthMaker.TryMakeFilth(parent.pawn.PositionHeld, parent.pawn.Map, sexPart.GetPartComp().Fluid.filth);
                }
            }
        }

    }
}
