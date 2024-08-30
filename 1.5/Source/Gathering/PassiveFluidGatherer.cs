using Cumpilation.Common;
using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
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


        public override void CompTick()
        {
            base.CompTick();
            
            PassiveFluidGathererCompProperties properties = (props as PassiveFluidGathererCompProperties);

            if (parent.IsHashIntervalTick(properties.tickIntervall))
            {
                //ModLog.Message("Ticking PassiveFluidGatherer happily");
                var filths = GetNearbyFilth(false, properties.range);
                var sexFluidFilths = GetNearbyFilth(true, properties.range);
                ModLog.Message($"{parent.def}@{parent.PositionHeld}:Found {filths.Count()} filths and {sexFluidFilths.Count()} Fluid-Associated Filths in range {properties.range}");
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
    }
}
