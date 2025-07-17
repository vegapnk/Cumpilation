using Verse;

namespace Cumpilation.Leaking
{
	public class HediffCompProperties_LeakCum : HediffCompProperties
	{
		public float leakRate = 1;
		public float leakMult = 1;

		public HediffCompProperties_LeakCum()
		{
			compClass = typeof(HediffComp_LeakCum);
		}
	}
}
