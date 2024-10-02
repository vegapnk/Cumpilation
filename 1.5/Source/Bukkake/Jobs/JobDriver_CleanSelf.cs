using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;

namespace Cumpilation.Bukkake
{
    public class JobDriver_CleanSelf : JobDriver
    {
        float cleanAmount = 1f;//severity of a single cumHediff removed per cleaning-round; 1f = remove entirely
        int cleaningTime = 120;//ticks - 120 = 2 real seconds, 3 in-game minutes

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(pawn, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(delegate
            {
                List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
                return !hediffs.Exists(x => BukkakeUtility.IsSplashHediff(x));//fail if cum disappears - means that also all the cum is gone
            });

            InitOrUpdateCleaningTime();

            Toil cleaning = Toils_General.Wait(cleaningTime, TargetIndex.None);//duration of 
            cleaning.WithProgressBarToilDelay(TargetIndex.A);

            yield return cleaning;
            yield return new Toil()
            {
                initAction = delegate ()
                {
                    //get one of the cum hediffs, reduce its severity
                    Hediff hediff =  BukkakeUtility.SplashesOnPawn(pawn).FirstOrFallback(null);

                    if (hediff != null)
                        hediff.Severity -= cleanAmount;
                }
            };
            yield break;
        }


        protected void InitOrUpdateCleaningTime()
        {
            var splash = BukkakeUtility.SplashesOnPawn(pawn).FirstOrFallback(null);
            if (splash != null) cleaningTime = 
                    BukkakeUtility.TicksToCleanSplashFromSelf(splash, 2.0f);
        }

    }
}
