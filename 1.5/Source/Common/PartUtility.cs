using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Common
{
    public class PartUtility
    {
        /// <summary>
        /// This method fullfills 3 steps:
        /// 1. Sanity Checks, e.g. is the pawn null, is the pawn spawned, etc. 
        /// 2. Is the pawn valid, e.g. is it a men when men are allowed, is it an animal 
        /// 3. Find all relevant, targetted parts as specified. 
        /// 
        /// If 1 or 2 fail, an empty list is returned. 
        /// </summary>
        /// <param name="pawn">The pawn for which to find SexParts.</param>
        /// <param name="targetPenis">Consider Penises (using `RJW.PartHelper.Is_Penis`)</param>
        /// <param name="targetVagina">Consider Vaginas (using `RJW.PartHelper.Is_Vagina`)</param>
        /// <param name="targetBreast">Consider Breasts (using HomeBrew helper)</param>
        /// <param name="targetAnus">Consider Anus (using `RJW.PartHelper.Is_Anus`)</param>
        /// <param name="targetOther">Consider SexParts that are not in any other category</param>
        /// <param name="allowMen">Men are valid targets (Using RW.Gender)</param>
        /// <param name="allowWomen">Women are valid targets (using RW.Gender)</param>
        /// <param name="allowFutas">Futas are valid targets (using RJW.GenderUtility.IsFuta); Futa is checked before men and women.</param>
        /// <param name="allowAnimals">Animals are valid targets.</param>
        /// <param name="onlyFirst">only return the first (fitting) part.</param>
        /// <param name="needsFluid">only return parts that have fluid.</param>
        /// <param name="blockingHediffs">Return an empty list if any of the HediffDefs is present at a severity other than 0.</param>
        /// <returns>All targetted SexParts of the pawn, or an empty List on Errors.</returns>
        public static IEnumerable<ISexPartHediff> FindFittingSexParts(Pawn pawn, 
            bool targetPenis = false, bool targetVagina = false, bool targetBreast = false, bool targetAnus = false, bool targetOther = false, 
            bool allowMen = true, bool allowWomen = true, bool allowFutas = true, 
            bool allowAnimals=false, bool onlyFirst = false, bool needsFluid = true,
            IEnumerable<HediffDef> blockingHediffs = null)
        {
            List<ISexPartHediff> results = new List<ISexPartHediff>();

            if (pawn == null) return results;
            if (pawn.IsAnimal() && !allowAnimals) return results;

            if (Genital_Helper.is_futa(pawn) && !allowFutas) { return results; }
            if (pawn.gender == Gender.Male && !allowMen) { return results; }
            if (pawn.gender == Gender.Female && !allowWomen) { return results; }

            // If there are any blocking Hediffs present, with a different severity than 0, return an empty list
            if (blockingHediffs != null && blockingHediffs.Count() > 0)
            {
                if (blockingHediffs.Any(bHediff => pawn.health.hediffSet.hediffs.Any(pHediff => pHediff.Severity != 0.0 && pHediff.def == bHediff)))
                    return results;
            }

            foreach (Hediff part in Genital_Helper.get_AllPartsHediffList(pawn))
            {
                if (results.Count > 0 && onlyFirst) { return results; }

                if (part is ISexPartHediff sexPart)
                {
                    if (sexPart.GetPartComp().Fluid == null && needsFluid) { continue; }

                    if (targetPenis && Genital_Helper.is_penis(part))
                    {
                        results.Add(sexPart);
                        continue;
                    }
                    else if (targetVagina && Genital_Helper.is_vagina(part))
                    {
                        results.Add(sexPart);
                        continue;
                    }
                    else if (targetBreast && IsBreasts(part))
                    {
                        results.Add(sexPart);
                        continue;
                    }
                    else if (targetAnus && Genital_Helper.is_anus(part))
                    {
                        results.Add(sexPart);
                        continue;
                    }
                    else if (targetOther)
                    {
                        results.Add(sexPart);
                        continue;
                    }
                }
            }

            return results;
        }

        public static bool IsBreasts(Hediff def)
        {
            if (def == null) return false;
            if (!(def is ISexPartHediff)) return false;

            return def.def.defName.ToLower().Contains("breast") || def.def.defName.ToLower().Contains("udder");
        }
    }
}
