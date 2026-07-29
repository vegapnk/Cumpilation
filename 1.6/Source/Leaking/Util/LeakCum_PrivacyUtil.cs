using System.Collections.Generic;
using Verse;
using Cumpilation.Reactions;

namespace Cumpilation.Leaking
{
    public class LeakCum_PrivacyUtil
    {   
        public static bool CanSeeDeflateSpot(List<Pawn> exclude, IntVec3 pos, Map map, out Pawn pawn)
        {
            foreach (Thing item in GenRadial.RadialDistinctThingsAround(pos, map, 7f, useCenter: true))
            {
                if (item is Pawn p && !p.RaceProps.Animal && !p.RaceProps.IsMechanoid && !exclude.Contains(p))
                {
                    if (CumpilationLineOfSight(pos, p.Position, map))
                    {
                        pawn = p;
                        return true; 
                    }
                }
            }
            pawn = null;
            return false;
        }

        public static bool CumpilationLineOfSight(IntVec3 start, IntVec3 end, Map map)
        {
            if (!GenSight.LineOfSight(start, end, map))
            {
                return false;
            }

            foreach (IntVec3 c in GenSight.PointsOnLineOfSight(start, end))
            {
                List<Thing> thingList = map.thingGrid.ThingsListAtFast(c);
                for (int i = 0; i < thingList.Count; i++)
                {
                    if (SightBlockerCache.LOSBlockingDefs.Contains(thingList[i].def))
                    {
                        return false;
                    }
                }
            }

            return true; 
        }
    }
}