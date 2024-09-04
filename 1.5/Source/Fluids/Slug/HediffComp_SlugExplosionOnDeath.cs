using Cumpilation.Common;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Fluids.Slug
{
    public class HediffComp_SlugExplosionOnDeath : HediffComp
    {
        public HediffCompProperties_SlugExplosionOnDeath Props => (HediffCompProperties_SlugExplosionOnDeath)this.props;

        public void TriggerExplosion()
        {


            Pawn pawn = this.parent.pawn;
            if (pawn == null || pawn.Corpse == null)
            {
                ModLog.Warning("Tried to Trigger an explosion - but received null or not-dead pawn");
                return;
            }

            /// DevNote: Copied from DeathActionWorker_ToxCloud and DeathActionWorker_MinorExplosion from base.Rimworld
            ModLog.Debug($"Triggering Slug-Explosion for {this.parent.pawn}");
            float radius = Props.baseRadius;
            radius = Props.radiusMultipliedByBodySize ? radius * this.parent.pawn.BodySize : radius;
            radius = Props.radiusMultipliedBySeverity ? radius * this.parent.Severity : radius;

            if (Props.doToxicCloud)
                GenExplosion.DoExplosion(pawn.Corpse.Position, pawn.Corpse.Map, radius, DamageDefOf.ToxGas, pawn.Corpse, postExplosionGasType: new GasType?(GasType.ToxGas));
            if (Props.doFireExplosion)
                GenExplosion.DoExplosion(pawn.Corpse.Position, pawn.Corpse.Map, radius, DamageDefOf.Flame, pawn.Corpse);
        }
    }
}
