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
        public static IEnumerable<ISexPartHediff> FindFittingSexParts(Pawn pawn, 
            bool targetPenis = false, bool targetVagina = false, bool targetBreast = false, bool targetAnus = false, bool targetOther = false, 
            bool allowMen = true, bool allowWomen = true, bool allowFutas = true, 
            bool allowAnimals=false, bool onlyFirst = false, bool needsFluid = true)
        {
            List<ISexPartHediff> results = new List<ISexPartHediff>();

            if (pawn == null) return results;
            if (pawn.IsAnimal() && !allowAnimals) return results;

            if (Genital_Helper.is_futa(pawn) && !allowFutas) { return results; }
            if (pawn.gender == Gender.Male && !allowMen) { return results; }
            if (pawn.gender == Gender.Female && !allowWomen) { return results; }

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
