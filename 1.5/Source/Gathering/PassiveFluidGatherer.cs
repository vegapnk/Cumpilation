using Cumpilation.Common;
using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;
using static HarmonyLib.Code;

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

        public Dictionary<ThingDef,int> GatheredFilth = new Dictionary<ThingDef, int>();

        public override void CompTick()
        {
            base.CompTick();
            
            PassiveFluidGathererCompProperties properties = (props as PassiveFluidGathererCompProperties);

            if (parent.IsHashIntervalTick(properties.tickIntervall))
            {
                var sexFluidFilths = GetNearbyFilth(true, properties.range);
               // ModLog.Message($"{parent.def}@{parent.PositionHeld}:Found {filths.Count()} filths and {sexFluidFilths.Count()} Fluid-Associated Filths in range {properties.range}");
                CleanFilth(sexFluidFilths);
                FilthToItem();
            }

        }

        public void FilthToItem()
        {
            PassiveFluidGathererCompProperties properties = (props as PassiveFluidGathererCompProperties);

            var GatheredFilthCopy = GatheredFilth.ToDictionary(entry => entry.Key,entry => entry.Value);

            foreach (var kk in GatheredFilthCopy.Keys)
            {
                var fgDef = LookupGatheringDef(kk);
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

        public FluidGatheringDef? LookupGatheringDef(ThingDef filth)
        {
            return DefDatabase<FluidGatheringDef>.AllDefs
                .Where(def => def.canBeRetrievedFromFilth)
                .Where(def => def.fluidDef.filth == filth || def.filth == filth)
                .FirstOrFallback();
        }

        public void CleanFilth(IEnumerable<Filth> filths)
        {
            PassiveFluidGathererCompProperties properties = (props as PassiveFluidGathererCompProperties);

            foreach (var filth in filths.Where(IsSupportedFilthType))
            {
                // If the cleanChance is not met, we just look at the next.
                if ((new Random()).NextDouble() > properties.cleanChance) continue;
                
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
                if (properties.cleanAtmostOne) return;
            }
        }

        public IEnumerable<Filth> GetNearbyFilth(bool onlyFluidFilth = true, int range = 50)
        {
            var results = new List<Filth>();

            if (this.parent.Map == null) return results;
            if (this.parent.GetRoom() == null) return results;
            if (this.parent.PositionHeld == null) return results;
            if (this.parent.IsBrokenDown()) return results;

            var room = this.parent.GetRoom();
            var knownFluids = FluidUtility.GetAllSexFluidDefs();

            //ModLog.Debug($"Checking Room {room.ID}({room.Role}), knowing {knownFluids.Count()} Fluids");

            // DevNote: I first was checking the cells of the room, but I checked the Cleaning Methods from BaseRW and they use the CotnainedAndAdjacentThings. 
            // So I went with that too.
            return room.ContainedAndAdjacentThings
                .OfType<Filth>()
                .Cast<Filth>()
                .Where(filth => filth.PositionHeld.InHorDistOf(this.parent.PositionHeld,range))
                .Where(filth => !onlyFluidFilth || knownFluids.Any(fluid => fluid.filth == filth.def));

        }

        public bool IsSupportedFilthType(Filth filth)
        {
            PassiveFluidGathererCompProperties properties = (props as PassiveFluidGathererCompProperties);
            bool isInFluidDefsAsFilth = properties.supportedFluids.Select(fluid => fluid.filth).Any(f => f == filth.def);
            bool isInFluidGatheringDefsAsBackup = DefDatabase<FluidGatheringDef>.AllDefs.Where(def => def.canBeRetrievedFromFilth && def.filth != null).Select(def => def.filth).Any(f => f == filth.def);
            return isInFluidDefsAsFilth || isInFluidGatheringDefsAsBackup;
        }
    }
}
