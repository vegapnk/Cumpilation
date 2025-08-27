using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using rjw;
using Verse;

namespace Cumpilation.Leaking.MainTab
{
	public class PawnTable_Deflate : PawnTable_PlayerPawns
	{
		public PawnTable_Deflate(PawnTableDef def, Func<IEnumerable<Pawn>> pawnsGetter, int uiWidth, int uiHeight) : base(def, pawnsGetter, uiWidth, uiHeight) { }

		protected override IEnumerable<Pawn> LabelSortFunction(IEnumerable<Pawn> input)
		{
			foreach (Pawn p in input)
				p.UpdatePermissions();
			return input.OrderByDescending(p => (p.IsPrisonerOfColony || p.IsSlaveOfColony) != false).ThenBy(p => xxx.get_pawnname(p));
		}

		protected override IEnumerable<Pawn> PrimarySortFunction(IEnumerable<Pawn> input)
		{
			foreach (Pawn p in input)
				p.UpdatePermissions();
			return input;
		}
	}
}
