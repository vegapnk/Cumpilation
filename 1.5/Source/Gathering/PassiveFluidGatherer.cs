using Cumpilation.Common;
using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Gathering
{
    /// <summary>
    /// Unlike `FluidGatheringBuilding` that works upon nearby sex, the PassiveFluidGatherer is trying to clean nearby floors from Filth.
    /// Due to logic, they must be in the same room. 
    /// 
    /// Important: For this to work properly, the ThingWithComp needs to have a `<tickerType>Normal</tickerType>` to run CompTick
    /// </summary>
    public class PassiveFluidGatherer : ThingComp
    {
        /// <summary>
        /// Stores the currently running information on how much of which filth was soaked in. 
        /// Important: This will reset on loading a game. Currently I have no plan on changing this. 
        /// </summary>
        public Dictionary<ThingDef,int> GatheredFilth = new Dictionary<ThingDef, int>();

        private PassiveFluidGathererCompProperties Props
        {
            get { return (PassiveFluidGathererCompProperties)this.props;}
        }

        //DevNote: I could also try to make a `CompTickRare`, then you need a `<tickerType>Rare</tickerType>` and the other Code is never run. But I don't know how that would go with HashIntervalTicks.
        public override void CompTick()
        {
            base.CompTick();

            if (parent.IsHashIntervalTick(Props.tickIntervall))
            {
                var sexFluidFilths = GetNearbyFilth(true, Props.range);
               // ModLog.Message($"{parent.def}@{parent.PositionHeld}:Found {filths.Count()} filths and {sexFluidFilths.Count()} Fluid-Associated Filths in range {properties.range}");
                CleanFilth(sexFluidFilths);
                FilthToItem();
            }
        }

        /// <summary>
        /// Checks the current `GatheredFilth` and spawns any item inside me if there is enough Filth gathered of the right kind.
        /// The logic for how much and what you need is done by `FluidGatheringDef`s. 
        /// </summary>
        public void FilthToItem()
        {
            // We need top make a copy to not get Runtime Errors about `Concurrent IEnumerable Modifications`
            var GatheredFilthCopy = GatheredFilth.ToDictionary(entry => entry.Key,entry => entry.Value);

            foreach (var kk in GatheredFilthCopy.Keys)
            {
                var fgDef = GatheringUtility.LookupGatheringDef(kk);
                if (fgDef == null) return;

                if (GatheredFilthCopy[kk] > fgDef.filthNecessaryForOneUnit) {
                    ModLog.Debug($"Filth {kk} in {this.parent}@{this.parent.PositionHeld} is enough to spawn {GatheredFilthCopy[kk] % fgDef.filthNecessaryForOneUnit} {fgDef.thingDef}'s");

                    Thing gatheredFluid = ThingMaker.MakeThing(fgDef.thingDef);
                    gatheredFluid.stackCount = GatheredFilthCopy[kk] % fgDef.filthNecessaryForOneUnit;
                    GenPlace.TryPlaceThing(gatheredFluid, this.parent.PositionHeld, this.parent.Map, ThingPlaceMode.Direct, out Thing res);

                    GatheredFilth[kk] -= gatheredFluid.stackCount * fgDef.filthNecessaryForOneUnit;
                }
            }
        }

        /// <summary>
        /// Sucks up all supported Filths (determined by the Def in PassiveFluidGathererCompProperties) and stores them in the GatheredFilth.
        /// Depending on your settings, there is a chance for filth to disappear or that only one thing at a time is cleaned. 
        /// </summary>
        /// <param name="filths">A list of all Filths to be considered.</param>
        public void CleanFilth(IEnumerable<Filth> filths)
        {
            foreach (var filth in filths.Where(IsSupportedFilthType))
            {
                // If the cleanChance is not met, we just look at the next.
                if ((new Random()).NextDouble() > Props.cleanChance) continue;
                
                filth.DeSpawn();
                if (GatheredFilth.ContainsKey(filth.def))
                {
                    GatheredFilth[filth.def] = filth.stackCount + GatheredFilth[filth.def];
                }
                else
                {
                    GatheredFilth.Add(filth.def, filth.stackCount);
                }
                // If you cleaned one, and you only clean one, you end this function.
                if (Props.cleanAtmostOne) return;
            }
        }

        /// <summary>
        /// Returns a list of all Filths that are within a given range, and within the same room of this Comps parent.
        /// When `onlyFluidFilth` is true, only filth that is specified in a rjw.SexFluidDef will be considered. 
        /// </summary>
        /// <param name="onlyFluidFilth">If true, only Filth that is mentioned in rjw.SexFluidDefs are returned</param>
        /// <param name="range">Filter for the horicontal distance to the parent.</param>
        /// <returns>A list of Fitlhs in the same room of the parent, filtered for SexFluids and/or range.</returns>
        public IEnumerable<Filth> GetNearbyFilth(bool onlyFluidFilth = true, int range = 50)
        {
            var results = new List<Filth>();

            if (this.parent.Map == null) return results;
            if (this.parent.GetRoom() == null) return results;
            if (this.parent.PositionHeld == null) return results;
            if (this.parent.IsBrokenDown()) return results;

            var room = this.parent.GetRoom();
            var knownFluids = FluidUtility.GetAllSexFluidDefs();

            // DevNote: I first was checking the cells of the room, but I checked the Cleaning Methods from BaseRW and they use the CotnainedAndAdjacentThings. 
            // So I went with that too.
            return room.ContainedAndAdjacentThings
                .OfType<Filth>()
                .Cast<Filth>()
                .Where(filth => filth.PositionHeld.InHorDistOf(this.parent.PositionHeld,range))
                .Where(filth => !onlyFluidFilth || knownFluids.Any(fluid => fluid.filth == filth.def));
        }

        /// <summary>
        /// Small helper that checks for a filth if the Gatherer can handle is. 
        /// This consist of checking if the filth is in a (supported) fluidDef, 
        /// or if there is a filth specified in a gatheringDef. 
        /// </summary>
        /// <param name="filth"></param>
        /// <returns></returns>
        public bool IsSupportedFilthType(Filth filth)
        {
            bool isInFluidDefsAsFilth = Props.supportedFluids.Select(fluid => fluid.filth).Any(f => f == filth.def);
            //TODO: Make this check a bit better if the gathering-defs fluid is also supported ...
            bool isInFluidGatheringDefsAsBackup = DefDatabase<FluidGatheringDef>.AllDefs
                .Where(def => def.canBeRetrievedFromFilth && def.filth != null)
                .Select(def => def.filth)
                .Any(f => f == filth.def);
            return isInFluidDefsAsFilth || isInFluidGatheringDefsAsBackup;
        }

    }
}
