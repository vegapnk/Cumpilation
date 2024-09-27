using HarmonyLib;
using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace Cumpilation.Bukkake
{
    public class BukkakeUtility
    {

        public static void CumOn(Pawn receiver, BodyPartRecord bodyPart, float amount = 0.2f, Pawn giver = null)
        {
            Hediff_Cum hediff = null;
            // TODO: find right type from Fluid ... 

            hediff.Severity = amount;//if this body part is already maximally full -> spill over to other parts

            //idea: here, a log entry that can act as source could be linked to the hediff - maybe reuse the playlog entry of rjw:
            try
            {
                //error when adding to missing part
                receiver.health.AddHediff(hediff, bodyPart, null, null);
            }
            catch
            {

            }
            //always also add cumcontroller hediff as manager
            //receiver.health.AddHediff(HediffDefOf.Hediff_CumController);
        }

        //determines who is the active male (or equivalent) in the exchange and the amount of cum dispensed and where to
        //[SyncMethod]
        public static void CalculateAndApplyCum(Pawn giver, Pawn receiver, SexProps props)
        {
            if (!Settings.EnableBukkake) return;

            ModLog.Debug($"Firing Bukkake Code {giver} ==> {receiver}");
            /*
            List<Hediff> giverparts;
            List<Hediff> pawnparts;
            List<Hediff> partnerparts = partner != pawn ? partner.GetGenitalsList() : null; // masturbation



            float cumAmount = giver.BodySize; //fallback for mechanoinds and w/e without hediffs
            float horniness = 1f;
            float ageScale = Math.Min(80 / SexUtility.ScaleToHumanAge(giver), 1.0f);//calculation lifted from rjw

            if (xxx.is_mechanoid(giver) && giverparts.NullOrEmpty())
            {
                //use default above
            }
            else if (giverparts.NullOrEmpty())
                return;
            else
            {
                var penisHediff = giverparts.FindAll((Hediff hed) => hed.def.defName.ToLower().Contains("penis")).InRandomOrder().FirstOrDefault();

                if (penisHediff == null)
                    penisHediff = giverparts.FindAll((Hediff hed) => hed.def.defName.ToLower().Contains("ovipositorf")).InRandomOrder().FirstOrDefault();
                if (penisHediff == null)
                    penisHediff = giverparts.FindAll((Hediff hed) => hed.def.defName.ToLower().Contains("ovipositorm")).InRandomOrder().FirstOrDefault();
                if (penisHediff == null)
                    penisHediff = giverparts.FindAll((Hediff hed) => hed.def.defName.ToLower().Contains("tentacle")).InRandomOrder().FirstOrDefault();

                if (penisHediff != null)
                {
                    cumAmount = penisHediff.Severity * giver.BodySize;

                    HediffComp_SexPart chdf = penisHediff.TryGetComp<rjw.HediffComp_SexPart>();
                    if (chdf != null)
                    {
                        if (chdf.FluidAmount != 0)
                            cumAmount = chdf.FluidAmount;
                    }
                    //ModLog.Message("cumAmount base " + cumAmount);

                    Need sexNeed = giver?.needs?.AllNeeds.Find(x => string.Equals(x.def.defName, "Sex"));
                    if (sexNeed != null)//non-humans don't have it - therefore just use the default value
                    {
                        horniness = 1f - sexNeed.CurLevel;
                    }
                }
                else
                {
                    //something is wrong... vagina?
                    return;
                }
            }

            cumAmount *= Settings.GlobaleBukkakeModifier;
            if (receiver != null)
            {
                List<BodyPartRecord> targetParts = FindFittingBodyParts(receiver,props);//which to apply cum on
                BodyPartRecord randomPart;//not always needed


                if (cumAmount > 0)
                {
                    cumOn(giver, genitals, cumAmount * 0.3f, giver, entityType);//cum on self - smaller amount
                    if (receiver != null)
                        foreach (BodyPartRecord bpr in targetParts)
                        {
                            if (bpr != null)
                            {
                                cumOn(receiver, bpr, cumAmount, giver, entityType);//cum on partner
                            }
                        }
                    
                }
            }
            */
        }

        public static HediffDef LookupCoverageHediff(Hediff sexPart) => sexPart is ISexPartHediff ? LookupCoverageHediff((ISexPartHediff)sexPart) : null;

        public static HediffDef LookupCoverageHediff(ISexPartHediff sexPart) => sexPart.GetPartComp().Fluid != null ? LookupCoverageHediff(sexPart.GetPartComp().Fluid) : null; 

        public static HediffDef LookupCoverageHediff(SexFluidDef fluid) {
            if (fluid == null) return null;

            return DefDatabase<HediffDef>.AllDefs
                .Where(def => def.comps.Any(comp => comp is HediffCompProperties_BukkakeSpawnedByFluid))
                .Where(def => {
                    HediffCompProperties_BukkakeSpawnedByFluid compProps = 
                        (HediffCompProperties_BukkakeSpawnedByFluid) def.comps.First(comp => comp is HediffCompProperties_BukkakeSpawnedByFluid);
                    return compProps.sexFluidDefs.Contains(fluid);
                })
                .FirstOrDefault(null);
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
            ModLog.Debug($"Got {targetParts.Count} for {receiver} pawns Bukkake with {props.sexType}");
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
