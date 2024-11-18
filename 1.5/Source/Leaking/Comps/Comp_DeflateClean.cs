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
    public class Comp_DeflateClean : ThingComp
    {
        private CompProperties_DeflateClean Props => (CompProperties_DeflateClean)props;
        public float deflateRate => Props.deflateRate;

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if (!selPawn.health.hediffSet.HasHediff(DefOfs.Cumpilation_Cumflation))
            {
                yield return new FloatMenuOption($"Deflate into {parent.Label} (not inflated)", null);
                yield break;
            }
            yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption($"Deflate into {parent.Label}", delegate
            {
                Job job = JobMaker.MakeJob(DefOfs.DeflateClean, parent);
                selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }), selPawn, parent);
        }
    }
}
