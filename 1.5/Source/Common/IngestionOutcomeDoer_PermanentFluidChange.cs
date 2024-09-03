using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Common
{
    public class IngestionOutcomeDoer_PermanentFluidChange : IngestionOutcomeDoer
    {

        SexFluidDef fluid;

        public bool changePenis = false;
        public bool changeVagina = false;
        public bool changeBreast = false;
        public bool changeOther = false;

        public bool onlyChangeOne = false;

        /// <summary>
        /// This is an internal reference used for follow up in inheriting methods. 
        /// Main reason: The slug-injection will give you intoxication if it cannot be applied. 
        /// </summary>
        protected bool wasSuccessfullyApplied = false;

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
        {
            if (pawn == null) return;
            ModLog.Debug($"{pawn} is consuming {ingestedCount} counts of {ingested}");

            foreach (Hediff part in Genital_Helper.get_AllPartsHediffList(pawn))
            {
                if (part is ISexPartHediff sexPart)
                {
                    if (changePenis && Genital_Helper.is_penis(part) && sexPart.GetPartComp().Fluid != fluid)
                    {
                        sexPart.GetPartComp().Fluid = fluid;
                        wasSuccessfullyApplied = true;
                        if (onlyChangeOne) return;
                    }
                    else if (changeVagina && Genital_Helper.is_vagina(part) && sexPart.GetPartComp().Fluid != fluid)
                    {
                        sexPart.GetPartComp().Fluid = fluid;
                        wasSuccessfullyApplied = true;
                        if (onlyChangeOne) return;
                    }
                    else if (changeBreast && (part.def.defName.ToLower().Contains("breast") || part.def.defName.ToLower().Contains("udder")) && sexPart.GetPartComp().Fluid != fluid)
                    {
                        sexPart.GetPartComp().Fluid = fluid;
                        wasSuccessfullyApplied = true;
                        if (onlyChangeOne) return;
                    }
                    else if (changeOther && sexPart.GetPartComp().Fluid != fluid)
                    {
                        sexPart.GetPartComp().Fluid = fluid;
                        wasSuccessfullyApplied = true;
                        if (onlyChangeOne) return;
                    }
                }
            }
        }

    }
}
