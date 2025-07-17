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
    public class PassiveFluidGathererComp : ThingComp
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
                var sexFluidFilths = GatheringUtility.GetNearbyFilth(this.parent,true, Props.range);
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

            foreach (var fluidFilthType in GatheredFilthCopy.Keys)
            {
                var fgDef = GatheringUtility.LookupGatheringDef(fluidFilthType);
                if (fgDef == null) return;

                if (GatheredFilthCopy[fluidFilthType] > fgDef.filthNecessaryForOneUnit) {
                    ModLog.Debug($"Filth {fluidFilthType} in {this.parent}@{this.parent.PositionHeld} is enough to spawn {GatheredFilthCopy[fluidFilthType] % fgDef.filthNecessaryForOneUnit} {fgDef.thingDef}'s");

                    Thing gatheredFluid = ThingMaker.MakeThing(fgDef.thingDef);
                    gatheredFluid.stackCount = GatheredFilthCopy[fluidFilthType] % fgDef.filthNecessaryForOneUnit;
                    GenPlace.TryPlaceThing(gatheredFluid, this.parent.PositionHeld, this.parent.Map, ThingPlaceMode.Direct, out Thing res);

                    // DevNote: I initially reduced it by the amount, but ... that lead to a weird bug spawning more and more and not resetting anything.
                    // So I went with this which is way more robust.
                    GatheredFilth[fluidFilthType] = 0;
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
            foreach (var filth in filths.Where(filth => GatheringUtility.IsSupportedFilthType(filth,Props.supportedFluids)))
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

    }
}
