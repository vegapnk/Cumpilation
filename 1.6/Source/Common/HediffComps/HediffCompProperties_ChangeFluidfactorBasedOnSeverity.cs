namespace Cumpilation.Common
{
    public class HediffCompProperties_ChangeFluidfactorBasedOnSeverity : HediffCompProperties_PartTargetting
    {
        public float max;
        public float min;

        public HediffCompProperties_ChangeFluidfactorBasedOnSeverity() => this.compClass = typeof(HediffComp_ChangeFluidfactorBasedOnSeverity);
    }

}
