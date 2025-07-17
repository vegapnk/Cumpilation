using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Cumpilation.Bukkake
{
    public class Hediff_CoverageController : HediffWithComps
    {
        public override bool TryMergeWith(Hediff other)
        {
            if (other == null || other.def != this.def)
            {
                return false;
            }
            return true;
        }

        public override void Tick()
        {
            base.Tick();

            // For Performance: Only check every our for reduction. 
            if (pawn.IsHashIntervalTick(60000/24))
                CalculateSeverity();
        }

        public void CalculateSeverity()
        {
            HediffCompProperties_SpawnOrAdjustControllerHediff spawnProps = LookupMySpawnProperties();
            if (spawnProps == null)
            {
                ModLog.Debug($"Failed to find spawnProps for {this.def.defName}");
                return;
            }

            float totalSplashSeverity = FindHediffsThatSpawnController(this.def, pawn).Sum(hed => hed.Severity);

            this.Severity = totalSplashSeverity / spawnProps.summedSeverityRequiredForFullController;
        }

        public static List<HediffDef> FindHediffDefsThatSpawnController(HediffDef controller)
        {
            return DefDatabase<HediffDef>.AllDefsListForReading
                .Where(
                  hed => hed.HasComp(typeof(HediffComp_SpawnOrAdjustControllerHediff))
                )
                .Where(
                  hed => hed.comps.Any(comp => comp is HediffCompProperties_SpawnOrAdjustControllerHediff && ((HediffCompProperties_SpawnOrAdjustControllerHediff)comp).controller == controller)
                )
                .ToList();
        }

        public static List<Hediff> FindHediffsThatSpawnController(HediffDef controller, Pawn carrier)
        {
            if (carrier == null || controller == null) return new List<Hediff>();

            var sources = FindHediffDefsThatSpawnController(controller);

            return carrier.health
                .hediffSet.hediffs
                .FindAll(hed => sources.Contains(hed.def));
        }

        HediffCompProperties_SpawnOrAdjustControllerHediff LookupMySpawnProperties()
        {
            var exampleSpawner = FindHediffDefsThatSpawnController(this.def).FirstOrFallback(null);
            if (exampleSpawner == null) return null;
            var comp = exampleSpawner.comps
                .First(comp => comp is HediffCompProperties_SpawnOrAdjustControllerHediff && ((HediffCompProperties_SpawnOrAdjustControllerHediff)comp).controller == this.def);

            return (HediffCompProperties_SpawnOrAdjustControllerHediff) comp;
        }
    }
}
