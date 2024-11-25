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
using Cumpilation;
using Cumpilation.Gathering;

namespace Cumpilation.Leaking
{
    class JobDriver_DeflateClean : JobDriver
    {
        public int ticksToDeflate = -1000;
        public float deflateRate = 1f;
        public Hediff cumflationHediff;
        public List<Pawn> seenBy = new List<Pawn>();

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(TargetIndex.A).Thing, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            AddFinishAction(delegate
            {
                if (pawn.health.hediffSet.HasHediff(DefOfs.Deflating))
                {
                    pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(DefOfs.Deflating));
                }
            });
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell);
            float initialSeverity = CumflationUtility.GetOrCreateCumflationHediff(pawn).Severity;
            deflateRate = (TargetA.Thing?.TryGetComp<Comp_DeflateBucket>()?.deflateRate ?? TargetA.Thing?.TryGetComp<Comp_DeflateClean>()?.deflateRate) ?? 1f;
            //ModLog.Debug($"Deflate rate = {deflateRate}");
            Toil deflate = ToilMaker.MakeToil("MakeNewToils");
            cumflationHediff = CumflationUtility.GetOrCreateCumflationHediff(pawn);
            seenBy = new List<Pawn>{pawn};
            deflate.tickAction = delegate
            {
                if (ShouldEnd())
                {
                    base.ReadyForNextToil();
                }
                if (ticksToDeflate < -100)
                {
                    ResetTicksToDeflate();
                }
                pawn.GainComfortFromCellIfPossible();
                ticksToDeflate--;
                if (ticksToDeflate <= 0)
                {
                    cumflationHediff.Severity -= 0.01f;
                    if (Settings.EnablePrivacy) CheckPrivacy();
                    DoDeflate();
                    ResetTicksToDeflate();
                }
            };
            deflate.initAction = delegate
            {
                pawn.health.AddHediff(DefOfs.Deflating);
            };
            deflate.defaultCompleteMode = ToilCompleteMode.Never;
            deflate.PlaySustainerOrSound(() => SoundDefOf.Recipe_Surgery);
            deflate.WithEffect(EffecterDefOf.Vomit, TargetIndex.A, new Color(100f, 100f, 100f, 0.5f));
            deflate.WithProgressBar(TargetIndex.A, () => 1f - cumflationHediff.Severity / initialSeverity);
            yield return deflate;
        }

        private void ResetTicksToDeflate()
        {
            float num = GetAverageLooseness() + CumflationUtility.GetOrCreateCumflationHediff(pawn).Severity + 0.5f;
            num *= Settings.DeflateRate * deflateRate;
            ModLog.Debug($"Deflate in {25f / num} ticks");
            ticksToDeflate = (int)Math.Round(25f / num);
        }

        private float GetAverageLooseness()
        {
            float num = 0f;
            int count = 1;
            foreach (var part in rjw.Genital_Helper.get_AllPartsHediffList(pawn))
            {
                if (rjw.Genital_Helper.is_vagina(part))
                {
                    num += part.Severity;
                    count++;
                }
            }
            return num / count;
        }

        private bool ShouldEnd()
        {
            return cumflationHediff == null || cumflationHediff.Severity <= 0f;
        }

        public virtual void DoDeflate()
        {

        }
        public void CheckPrivacy()
        {
            if (pawn.IsPrisoner)
            {
                return;
            }
            if (pawn.AnimalOrWildMan())
            {
                return;
            }
            List<Pawn> list = new List<Pawn>{pawn};
            if (LeakCum_PrivacyUtil.CanSeeDeflateSpot(seenBy ,pawn.Position, pawn.MapHeld, out Pawn peeker) && peeker != pawn && !seenBy.Contains(peeker))
            {
                pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(DefOfs.Cumpilation_Thought_SeenWhenDeflating);
                seenBy.Add(peeker);
            }
        }
    }
}