using System.Collections.Generic;
using Verse;
using RimWorld;
using Verse.Noise;
using RimWorld.QuestGen;
using rjw;
using Cumpilation.Cumflation;
using Cumpilation;
using Cumpilation.Gathering;

namespace Cumpilation.Leaking
{
	public class HediffComp_LeakCum : HediffComp
	{
		private HediffCompProperties_LeakCum Props => (HediffCompProperties_LeakCum)props;
		private float amountLeaked = 0f;

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (!(parent.pawn.TryGetComp(out Comp_SealCum comp) && comp.IsSealed()))
			{
				float num = (parent.Severity + GetAverageLooseness() + 0.05f) * Props.leakRate * Settings.LeakRate * 0.004f;
				if (Rand.Chance(num))
				{
					parent.Severity -= 0.005f;
                    if (parent.pawn.MapHeld == null) return;	//No filth if pawn is out of map.
                    DropCumFilth();
				}
			}
		}

		private float GetAverageLooseness()
		{
			float num = 0f;
			int count = 1;
			foreach (var part in rjw.Genital_Helper.get_AllPartsHediffList(parent.pawn))
			{
				if (rjw.Genital_Helper.is_vagina(part))
				{
					num += part.Severity;
					count++;
				}
			}
			return num / count;
		}

		private void DropCumFilth()
		{
			if (!Settings.EnableFilthGeneration) { return; }
            rjw.SexFluidDef fluid;
			IEnumerable<FluidSource> sources = parent.TryGetComp<HediffComp_SourceStorage>().sources;
			if (sources.TryRandomElementByWeight(source => source.amount, out FluidSource chosenFluid))
			{
				fluid = chosenFluid.fluid;
			}
			else
			{
				fluid = DefOfs.Cum;
			}
			amountLeaked += CumflationUtility.FluidAmountRequiredToCumflatePawn(parent.pawn, fluid) * 0.005f * Props.leakMult * Settings.LeakMult;
			FluidGatheringDef fgDef = GatheringUtility.LookupFluidGatheringDef(fluid);
			while (amountLeaked >= fgDef.fluidRequiredForOneUnit / fgDef.filthNecessaryForOneUnit)
			{
				FilthMaker.TryMakeFilth(parent.pawn.PositionHeld, parent.pawn.MapHeld, fluid.filth);
				amountLeaked -= fgDef.fluidRequiredForOneUnit / fgDef.filthNecessaryForOneUnit;
			}
		}
	}
}
