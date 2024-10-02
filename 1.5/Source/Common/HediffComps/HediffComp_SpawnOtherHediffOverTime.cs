using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Common
{
    public class HediffComp_SpawnOtherHediffOverTime : HediffComp
    {
        public HediffCompProperties_SpawnOtherHediffOverTime Props => (HediffCompProperties_SpawnOtherHediffOverTime)this.props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if (this.props != null && Pawn.IsHashIntervalTick(Props.tickInterval) && Props.IsValidPawn(Pawn)) { 
                if ((new Random()).NextDouble() <= Props.applicationChance){

                    Hediff spawnedHediff = Pawn.health.hediffSet.GetFirstHediffOfDef(Props.hediff);
                    if (spawnedHediff == null)
                    {
                        BodyPartRecord bpr = Props.TryToSpawnInSameBodyPart ? parent.Part : null;
                        Hediff hediff = HediffMaker.MakeHediff(Props.hediff, Pawn, bpr);
                    }
                    spawnedHediff.Severity += Props.severityIncrease;
                }
            }
        }
    }
}
