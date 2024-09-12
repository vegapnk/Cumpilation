using RimWorld;
using Verse;

namespace Cumpilation.Fluids.Cum
{
    /// <summary>
    /// (Simple) Filters for Food to include or non-include cum. 
	/// 
    /// DevNote: Gently taken from Amevarashi-Sexperience
    /// https://gitgud.io/amevarashi/rjw-sexperience/-/tree/master/Source/RJWSexperience/Cum/FilterWorkers?ref_type=heads
    /// </summary>
    public abstract class SpecialThingFilterWorker_CumBase : SpecialThingFilterWorker
	{
		public override bool CanEverMatch(ThingDef def)
		{
			return def.IsIngestible && def.IsProcessedFood;
		}

		protected bool IsCum(ThingDef t) => t == DefOfs.Cumpilation_Cum;

		protected bool IsFoodWithCum(Thing food)
		{
			CompIngredients compIngredients = food.TryGetComp<CompIngredients>();

			if (compIngredients?.ingredients == null)
				return false;

			for (int i = 0; i < compIngredients.ingredients.Count; i++)
			{
				if (IsCum(compIngredients.ingredients[i]))
					return true;
			}

			return false;
		}
	}
}
