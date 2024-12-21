using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using rjw;
using UnityEngine;
using Verse;

namespace Cumpilation.Leaking.MainTab
{
	[StaticConstructorOnStartup]
	public class PawnColumnWorker_AllowDeflate : PawnColumnWorker_Checkbox
	{
		protected override bool HasCheckbox(Pawn pawn)
		{
			return pawn.DevelopmentalStage == DevelopmentalStage.Adult && Genital_Helper.has_vagina(pawn);
		}

		protected override bool GetValue(Pawn pawn)
		{
			return pawn.TryGetComp(out Comp_SealCum comp) && comp.CanDeflate();
		}

		protected override void SetValue(Pawn pawn, bool value, PawnTable table)
		{
			if (value == this.GetValue(pawn)) return;
			
			pawn.TryGetComp(out Comp_SealCum comp);
            comp.ToggleDeflate();
        }
	}
}
