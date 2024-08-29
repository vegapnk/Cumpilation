using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Gathering
{
    public static class PawnExtensions
    {
        public static T GetAdjacentBuilding<T>(this Pawn pawn) where T : Building => GetAdjacentBuildings<T>(pawn).FirstOrFallback();

        public static IEnumerable<T> GetAdjacentBuildings<T>(this Pawn pawn) where T : Building
        {
            if (!pawn.Spawned)
                yield break;

            EdificeGrid edifice = pawn.Map.edificeGrid;
            if (edifice[pawn.Position] is T building)
                yield return building;

            foreach (IntVec3 pos in GenAdjFast.AdjacentCells8Way(pawn.Position))
            {
                if (pos.InBounds(pawn.Map) && edifice[pos] is T adjBuilding)
                    yield return adjBuilding;
            }
        }
    }
}
