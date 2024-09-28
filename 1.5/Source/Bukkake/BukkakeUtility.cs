using Cumpilation.Common;
using HarmonyLib;
using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace Cumpilation.Bukkake
{
    public class BukkakeUtility
    {

        public static void CumOn(Pawn receiver, BodyPartRecord bodyPart, SexFluidDef fluid, float amount, Pawn giver)
        {
            var HediffDef = LookupCoverageHediff(fluid);
            if (HediffDef == null)
            {
                ModLog.Warning($"Tried to cum on {receiver} with {fluid}, did not find a matching coverage hediff");
                return;
            }

            HediffCompProperties_BukkakeSpawnedByFluid spawnProps =
                        (HediffCompProperties_BukkakeSpawnedByFluid) HediffDef.comps.First(comp => comp is HediffCompProperties_BukkakeSpawnedByFluid);
            if (spawnProps == null) return;

            float totalSeverityToSpawn = amount / spawnProps.fluidRequiredForSeverityOne;
            if (totalSeverityToSpawn <= 0.1) return;

            ModLog.Debug($"Spawning {totalSeverityToSpawn} severity of {HediffDef.defName} on {receiver}s {bodyPart.def.defName}");

            Hediff splash = HediffMaker.MakeHediff(HediffDef, receiver, bodyPart);
            splash.Severity = Math.Min(totalSeverityToSpawn,1.0f);
            receiver.health.AddHediff(splash);
            totalSeverityToSpawn -= 1.0f;

            //TODO: Overflow Spilling improvements - only goes once ... 
            while (totalSeverityToSpawn >= 0.1)
            {
                var spillPart = SpillOver(bodyPart.def);
                if (spillPart != null)
                {
                    var record = receiver.RaceProps.body.GetPartsWithDef(spillPart).RandomElementWithFallback(null);
                    if (record != null)
                    {
                        Hediff overSpillSplash = HediffMaker.MakeHediff(HediffDef, receiver, bodyPart);
                        overSpillSplash.Severity = Math.Min(totalSeverityToSpawn, 1.0f);
                        receiver.health.AddHediff(overSpillSplash);
                    }
                }
                totalSeverityToSpawn -= 1.0f;
            }


            if (HediffDef.HasComp(typeof(HediffComp_SpawnOrAdjustControllerHediff))) {
                HediffCompProperties_SpawnOrAdjustControllerHediff controllerProps = 
                    (HediffCompProperties_SpawnOrAdjustControllerHediff) HediffDef.comps.First(comp => comp is HediffCompProperties_SpawnOrAdjustControllerHediff);
                if (controllerProps == null || controllerProps.controller == null) return;
                Hediff controller = HediffMaker.MakeHediff(controllerProps.controller, receiver);
                //TODO: Calculate the Severity / Auto-Calculate it for the Controller
                receiver.health.AddHediff(controller);
                ModLog.Debug($"Spawned Controller {controller.def.defName} on {receiver} with {controller.Severity}");
            }
            
        }

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
                var randomPart = FindFittingBodyParts(receiver, props).RandomElementWithFallback(null);
                if (randomPart != null)
                {
                    ModLog.Debug($"{giver} is covering {receiver}s {randomPart} with {shootingGenital.Def.defName}");
                    CumOn(receiver, randomPart, fluid: shootingGenital.GetPartComp().Fluid, amount: shootingGenital.GetPartComp().FluidAmount, giver:giver);
                }
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


        //if spunk on one body part reaches a certain level, it can spill over to others, this function returns from where to where
        public static BodyPartDef SpillOver(BodyPartDef sourcePart)
        {
            //caution: danger of infinite loop if circular spillover between 2 full parts -> don't define possible circles
            BodyPartDef newPart = null;
            if (sourcePart == BodyPartDefOf.Torso)
            {
                newPart = Rand.Range(0, 3) switch
                {
                    0 => BodyPartDefOf.Arm,
                    1 => BodyPartDefOf.Leg,
                    2 => NeckDef,
                    3 => chestDef,
                    _ => null
                };
            }
            else if (sourcePart == JawDef)
            {
                newPart = Rand.Range(0, 3) switch
                {
                    0 => BodyPartDefOf.Head,
                    1 => BodyPartDefOf.Torso,
                    2 => NeckDef,
                    3 => chestDef,
                    _ => null
                };
            }
            else if (sourcePart == genitalsDef)
            {
                newPart = Rand.Range(0, 1) switch
                {
                    0 => BodyPartDefOf.Leg,
                    1 => BodyPartDefOf.Torso,
                    _ => null
                };
            }
            else if (sourcePart == anusDef)
            {
                newPart = Rand.Range(0, 1) switch
                {
                    0 => BodyPartDefOf.Leg,
                    1 => BodyPartDefOf.Torso,
                    _ => null
                };
            }
            else if (sourcePart == chestDef)
            {
                newPart = Rand.Range(0, 2) switch
                {
                    0 => BodyPartDefOf.Arm,
                    1 => BodyPartDefOf.Torso,
                    2 => NeckDef,
                    _ => null
                };
            }
            return newPart;
        }
    }
}
