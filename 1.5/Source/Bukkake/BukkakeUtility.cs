using Cumpilation.Common;
using HarmonyLib;
using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace Cumpilation.Bukkake
{
    /// <summary>
    /// Initializes and Manages both the "Per Body-Part" Splashes on Bukkake as well as their controllers. 
    /// 
    /// DevNote: 
    /// Initially, the Bukkake implementation from Ed86-Cum had two mechanics that got removed. 
    /// 1) The body-parts had a "Overspill" logic which means that if your chest was full, 
    ///    you'd get splashes on the arms and legs. 
    /// 2) The Controller would calculate itself regularly. 
    ///    This is now done upon spawning parts and the controller just ticks down.  
    /// 
    /// Also, all drawing got removed. The drawing framework people should take care of that (#Innocent)
    /// </summary>
    public class BukkakeUtility
    {
        public static void CalculateAndApplyCum(Pawn giver, Pawn receiver, SexProps props)
        {
            if (!Settings.EnableBukkake) return;
            if (!CanBeCovered(receiver)) return;
            if (!FluidUtility.IsSexWithFluidFlyingAround(props)) return;

            var giverGenitals =
                FluidUtility.GetGenitalsWithFluids(giver, true);

            if (giverGenitals.Count() > 0)
                ModLog.Debug($"{giver} tries to cover {receiver} with {giverGenitals.Count()} fitting genitals");
            else
                ModLog.Debug($"{giver} did not have qualified genitalia to cover {receiver}");

            foreach (ISexPartHediff shootingGenital in giverGenitals)
            {

                var sexPartComp = shootingGenital.GetPartComp();

                var splashDef = LookupCoverageHediff(sexPartComp.Fluid);
                if (splashDef == null)
                {
                    ModLog.Debug($"Tried to cum on {receiver} with {sexPartComp.Fluid} from {shootingGenital.Def}, did not find a matching coverage hediff");
                    continue;
                }

                HediffCompProperties_BukkakeSpawnedByFluid spawnProps =
                            (HediffCompProperties_BukkakeSpawnedByFluid)splashDef.comps.First(comp => comp is HediffCompProperties_BukkakeSpawnedByFluid);
                if (spawnProps == null) continue;

                float totalSeverityToSpawn = sexPartComp.FluidAmount / spawnProps.fluidRequiredForSeverityOne;
                if (totalSeverityToSpawn <= 0.1) continue;

                var targets = 
                    PartitionSeverity(totalSeverityToSpawn)
                    .Select(p => (p,FindFittingBodyParts(receiver,props).RandomElementWithFallback(null)))
                    .Where( (a,bpr) => bpr != null && bpr != 0);

                ModLog.Debug($"Split {giver}s cumshot of (total) {totalSeverityToSpawn} severity into {targets.Count()} sub-targets on {receiver}");
                foreach( (float,BodyPartRecord) target in targets)
                {
                    CumOn(receiver:receiver, splashDef:splashDef, bodyPart: target.Item2, severity:target.Item1, giver:giver);
                }

                AddAndCalculateControllers(splashDef, receiver);
            }
        }

        public static void CumOn(Pawn receiver, HediffDef splashDef, BodyPartRecord bodyPart, float severity, Pawn giver)
        {
            Hediff splash = HediffMaker.MakeHediff(splashDef, receiver, bodyPart);
            splash.Severity = Math.Min(severity, 1.0f);
            receiver.health.AddHediff(splash);
        }

        public static void AddAndCalculateControllers(HediffDef splashDef, Pawn carrier)
        {
            if (splashDef.HasComp(typeof(HediffComp_SpawnOrAdjustControllerHediff)))
            {
                HediffCompProperties_SpawnOrAdjustControllerHediff controllerProps =
                    (HediffCompProperties_SpawnOrAdjustControllerHediff) splashDef.comps.First(comp => comp is HediffCompProperties_SpawnOrAdjustControllerHediff);

                if (controllerProps == null || controllerProps.controller == null) return;

                Hediff controller = HediffMaker.MakeHediff(controllerProps.controller, carrier);
                if (controller is Hediff_CoverageController cumController) {
                    cumController.CalculateSeverity();
                    if (cumController.Severity <= 0.1) return;
                    carrier.health.AddHediff(cumController);
                    ModLog.Debug($"Spawned Controller {cumController.def.defName} on {carrier} with {cumController.Severity} severity");
                }
            } else
            {
                ModLog.Debug($"Did not find a controller-hediff for a bukkake-splash of def {splashDef.defName}");
            }
        }

        /// <summary>
        /// Simple check if the pawn can be covered with fluids.
        /// Only some sanity checks and ruling animals out.
        /// </summary>
        /// <param name="pawn">The pawn that might be covered.</param>
        /// <returns>True if the pawn can be covered by fluid, false otherwise.</returns>
        public static bool CanBeCovered(Pawn pawn)
        {
            if (pawn == null) return false;
            if (pawn.IsAnimal()) return false;
            if (!pawn.Spawned) return false;

            return true;
        }

        public static HediffDef LookupCoverageHediff(Hediff sexPart) => sexPart is ISexPartHediff ? LookupCoverageHediff((ISexPartHediff)sexPart) : null;
        public static HediffDef LookupCoverageHediff(ISexPartHediff sexPart) => sexPart.GetPartComp().Fluid != null ? LookupCoverageHediff(sexPart.GetPartComp().Fluid) : null; 
        public static HediffDef LookupCoverageHediff(SexFluidDef fluid) {
            if (fluid == null) return null;

            foreach (HediffDef def in DefDatabase<HediffDef>.AllDefs) {
                if (!def.HasComp(typeof(HediffComp_BukkakeSpawnedByFluid))) continue;

                HediffCompProperties_BukkakeSpawnedByFluid compProps =
                    (HediffCompProperties_BukkakeSpawnedByFluid)def.comps.FirstOrFallback(comp => comp is HediffCompProperties_BukkakeSpawnedByFluid,null);
                if (compProps == null) continue;

                if (compProps.sexFluidDefs.Contains(fluid))
                    return def;
            }
            return null;
        }

        /// <summary>
        /// Partitions a float into a sub-list of smaller floats whose sum is the initialSeverity. 
        /// There will be atmost 3 * initialseverity parts, or max_parts, whatever is smaller. 
        /// The size of each partition is capped at 1.0 (default).
        /// </summary>
        /// <param name="initialSeverity"></param>
        /// <param name="max_parts">Max number of partitions to create, default capped at 10</param>
        /// <param name="max_severity_on_part">Max value for each entry, default capped at 1.0</param>
        /// <returns>A list of floats whose sum is the original severity.</returns>
        public static List<float> PartitionSeverity(float initialSeverity, int max_parts = 10, float max_severity_on_part = 1.0f)
        {
            int parts = Math.Min( Convert.ToInt32(initialSeverity) * 3 + 1, max_parts);
            List<float> results = new List<float>();
            float remaining_severity = initialSeverity;
            var random = new System.Random();
            for (int i = 0; i < parts; i++)
            {
                float substract_severity = 
                    (float) (random.NextDouble() *initialSeverity); 
                substract_severity = Math.Min(substract_severity, max_severity_on_part);
                results.Add(substract_severity);
                remaining_severity -= substract_severity;
            }
            results.Add(remaining_severity);
            results.Shuffle();
            return results;
        }


        //get defs of the rjw parts
        public static BodyPartDef genitalsDef = BodyDefOf.Human.AllParts.Find(bpr => string.Equals(bpr.def.defName, "Genitals")).def;
        public static BodyPartDef anusDef = BodyDefOf.Human.AllParts.Find(bpr => string.Equals(bpr.def.defName, "Anus")).def;
        public static BodyPartDef chestDef = BodyDefOf.Human.AllParts.Find(bpr => string.Equals(bpr.def.defName, "Chest")).def;
        public static BodyPartDef JawDef = BodyDefOf.Human.AllParts.Find(bpr => string.Equals(bpr.def.defName, "Jaw")).def;
        public static BodyPartDef NeckDef = BodyDefOf.Human.AllParts.Find(bpr => string.Equals(bpr.def.defName, "Neck")).def;



        //all body parts that cum can theoretically be applied to:
        // DevNote: Intentionally a method, so that other mods could add parts to be bukkakee`d
        public static List<BodyPartDef> GetAllowedBodyParts()
        {
            return new List<BodyPartDef>()
            {BodyPartDefOf.Arm, BodyPartDefOf.Leg,BodyPartDefOf.Head,
                JawDef, NeckDef, BodyPartDefOf.Torso,
                genitalsDef, anusDef, chestDef
            };
        }

        //get valid body parts for a specific pawn
        public static IEnumerable<BodyPartRecord> GetAvailableBodyParts(Pawn pawn)
        {
            //get all non-missing body parts:
            IEnumerable<BodyPartRecord> bodyParts = pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Outside, null, null);
            BodyPartRecord anus = pawn.def.race.body.AllParts.Find(bpr => string.Equals(bpr.def.defName, "Anus"));//not found by above function since depth is "inside"

            if (anus != null)
            {
                bodyParts = bodyParts.AddItem(anus);
            }

            //filter for allowed body parts (e.g. no single fingers/toes):
            List<BodyPartDef> filterParts = GetAllowedBodyParts();

            IEnumerable<BodyPartRecord> filteredParts = bodyParts.Where(item1 => filterParts.Any(item2 => item2.Equals(item1.def)));
            return filteredParts;
        }


        public static List<BodyPartRecord> FindFittingBodyParts(Pawn receiver, SexProps props)
        {
            IEnumerable<BodyPartRecord> availableParts = GetAvailableBodyParts(receiver);
            var targetParts = new List<BodyPartRecord>();
            switch (props.sexType)
            {
                case rjw.xxx.rjwSextype.Anal:
                    targetParts.Add(receiver.RaceProps.body.AllParts.Find(x => x.def == anusDef));
                    break;
                case rjw.xxx.rjwSextype.Boobjob:
                    targetParts.Add(receiver.RaceProps.body.AllParts.Find(x => x.def == chestDef));
                    break;
                case rjw.xxx.rjwSextype.DoublePenetration:
                    targetParts.Add(receiver.RaceProps.body.AllParts.Find(x => x.def == anusDef));
                    targetParts.Add(receiver.RaceProps.body.AllParts.Find(x => x.def == genitalsDef));
                    break;
                case rjw.xxx.rjwSextype.Masturbation:
                case rjw.xxx.rjwSextype.Fingering:
                case rjw.xxx.rjwSextype.Fisting:
                case rjw.xxx.rjwSextype.None:
                    break;
                case rjw.xxx.rjwSextype.Footjob:
                case rjw.xxx.rjwSextype.Handjob:
                case rjw.xxx.rjwSextype.MutualMasturbation:
                    //random part:
                    targetParts.Add(availableParts.RandomElement());
                    break;

                case rjw.xxx.rjwSextype.Sixtynine:
                case rjw.xxx.rjwSextype.Oral:
                    targetParts.Add(receiver.RaceProps.body.AllParts.Find(x => x.def == JawDef));
                    break;
                case rjw.xxx.rjwSextype.Vaginal:
                case rjw.xxx.rjwSextype.Scissoring:
                    targetParts.Add(receiver.RaceProps.body.AllParts.Find(x => x.def == genitalsDef));
                    break;
                default: ModLog.Debug($"Tried to find fitting bukkake-parts for {receiver} when having {props.sexType} --- did not find a matching part"); break;
            }

            targetParts = targetParts.Where(f => f != null).ToList();
            //ModLog.Debug($"Got {targetParts.Count} for {receiver} pawns Bukkake with {props.sexType}");
            return targetParts;
        }

    }
}
