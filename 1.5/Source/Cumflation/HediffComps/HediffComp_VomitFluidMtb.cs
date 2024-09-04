using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Cumflation
{
    public class HediffComp_VomitFluidMtb : HediffComp
    {
        public HediffCompProperties_VomitFluidMtb Props => (HediffCompProperties_VomitFluidMtb) this.props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            // Do nothing on very small stuffings
            if (this.parent == null || this.parent.Severity <= 0.2) return;

            float chance = Props.scaleWithSeverity ? Props.mtbDays * (1/this.parent.Severity) : Props.mtbDays;

            if (chance > 0f && base.Pawn.IsHashIntervalTick(60) && Rand.MTBEventOccurs(chance, 60000f, 60f))
            {
                // TODO: LicentiaLab has Extra behaviour that Likes_Cumflation won't throw up
                this.Pawn.jobs.StartJob(JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("VomitFluid")), lastJobEndCondition: Verse.AI.JobCondition.InterruptForced, resumeCurJobAfterwards: true);
            }
        }

    }

}
