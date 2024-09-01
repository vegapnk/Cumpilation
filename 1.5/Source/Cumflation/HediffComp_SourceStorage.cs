using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Cumflation
{
    public class HediffComp_SourceStorage : HediffComp
    {
        public List<FluidSource> sources = new();


        public HediffCompProperties_SourceStorage Props => (HediffCompProperties_SourceStorage)this.props;


        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Collections.Look<FluidSource>(ref this.sources, "sources",lookMode:LookMode.Deep);
        }

        public void AddOrMerge(FluidSource incoming)
        {
            FluidSource existing = sources
                .Where(source => source.pawn == incoming.pawn && source.fluid == incoming.fluid)
                .FirstOrFallback();

            if (existing != null)
            {
                existing.amount += incoming.amount;
            }
            else
            {
                sources.Add(incoming);
                ModLog.Debug($"Added a new FluidSource to Cumflation-Hediff, now there are {sources.Count} sources in");
            }
        }

    }
}
