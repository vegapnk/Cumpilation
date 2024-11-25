// Assembly-CSharp, Version=1.5.9048.28434, Culture=neutral, PublicKeyToken=null
// RimWorld.JobGiver_GetRest
using System;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Cumpilation.Cumflation;
using System.Collections.Generic;
using System.Linq;

namespace Cumpilation.Leaking
{
    public class JobGiver_Deflate : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.TryGetComp(out Comp_SealCum comp) && !comp.CanDeflate())
            {
                return null;
            }
            if (Settings.EnableAutoDeflateBucket && TryFindBucketFor(pawn, out Thing bucket))
            {
                return JobMaker.MakeJob(DefOfs.DeflateBucket, bucket);
            }
            if (Settings.EnableAutoDeflateClean && TryFindCleanFor(pawn, out Thing clean))
            {
                return JobMaker.MakeJob(DefOfs.DeflateClean, clean);
            }
            if (Settings.EnableAutoDeflateDirty && TryFindDirtyFor(pawn, out IntVec3 cell))
            {
                return JobMaker.MakeJob(DefOfs.DeflateDirty, cell);
            }
            return null;
        }

        private bool TryFindBucketFor(Pawn pawn, out Thing thing)
        {
            List<Thing> things = new List<Thing>();
            foreach (ThingDef thingDef in ThingDefsDeflate.bucketDefs)
            {
                things.AddRange(pawn.MapHeld.listerThings.ThingsOfDef(thingDef));
            }
            thing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, things, PathEndMode.OnCell, TraverseParms.For(pawn), Settings.AutoDeflateMaxDistance, (Thing t) => (int)t.Position.GetDangerFor(pawn, pawn.MapHeld) <= (int)Danger.None, (Thing t) => priorityGetter(t));
            return thing != null;

            float priorityGetter(Thing t)
            {
                float priority = 0f;
                priority += (t.TryGetComp<Comp_DeflateBucket>()?.deflateMult ?? 1f) * 100f;
                if (Settings.EnablePrivacy && !LeakCum_PrivacyUtil.CanSeeDeflateSpot(new List<Pawn> { pawn }, t.Position, t.MapHeld, out Pawn p))
                {
                    priority += 10;
                }
                priority += t.TryGetComp<Comp_DeflateBucket>()?.deflateRate ?? 1f;
                return priority;
            }
        }

        private bool TryFindCleanFor(Pawn pawn, out Thing thing)
        {
            List<Thing> things = new List<Thing>();
            foreach (ThingDef thingDef in ThingDefsDeflate.cleanDefs)
            {
                things.AddRange(pawn.MapHeld.listerThings.ThingsOfDef(thingDef));
            }
            thing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, things, PathEndMode.OnCell, TraverseParms.For(pawn), Settings.AutoDeflateMaxDistance, (Thing t) => (int)t.Position.GetDangerFor(pawn, pawn.MapHeld) <= (int)Danger.None, (Thing t) => priorityGetter(t));
            return thing != null;

            float priorityGetter(Thing t)
            {
                float priority = 0f;
                if (Settings.EnablePrivacy && !LeakCum_PrivacyUtil.CanSeeDeflateSpot(new List<Pawn> { pawn }, t.Position, t.MapHeld, out Pawn p))
                {
                    priority += 10;
                }
                priority += t.TryGetComp<Comp_DeflateClean>()?.deflateRate ?? 1f;
                return priority;
            }
        }

        private bool TryFindDirtyFor(Pawn pawn, out IntVec3 cell)
        {
            Map map = pawn.Map;
            IntVec3 position = pawn.Position;
            if (IsValidCell(pawn, position))
            {
                cell = position;
                return true;
            }
            for (int i = 0; i < 2; i++)
            {
                int radius = (i == 0) ? 4 : 12;
                if (CellFinder.TryRandomClosewalkCellNear(position, map, radius, out var result, (IntVec3 c) => IsValidCell(pawn, c)))
                {
                    cell = result;
                    return true;
                }
            }
            cell = CellFinder.RandomClosewalkCellNearNotForbidden(pawn, 4, (IntVec3 c) => IsValidCell(pawn, c));
            return IsValidCell(pawn, cell);
        }

        private static bool IsValidCell(Pawn pawn, IntVec3 cell)
        {
            if (!cell.IsForbidden(pawn) && !cell.GetTerrain(pawn.Map).avoidWander)
            {
                return pawn.CanReserve(cell);
            }
            return false;
        }
    }
}