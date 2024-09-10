using Verse;

namespace Cumpilation.Fluids.Cum
{
	public class SpecialThingFilterWorker_Cum : SpecialThingFilterWorker_CumBase
	{
		public override bool Matches(Thing t)
		{
			return IsCum(t.def) || IsFoodWithCum(t);
		}
	}
}
