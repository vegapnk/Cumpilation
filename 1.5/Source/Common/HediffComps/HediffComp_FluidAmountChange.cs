using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Common
{
    public class HediffComp_FluidAmountChange : HediffComp
    {

        public HediffCompProperties_FluidAmountChange Props => (HediffCompProperties_FluidAmountChange)this.props;

        List<Hediff> storedHediffs = new List<Hediff>();

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);

            Pawn pawn = this.parent.pawn;

            foreach (Hediff part in Genital_Helper.get_AllPartsHediffList(pawn))
            {
                if (part is ISexPartHediff sexPart)
                {
                    if (Props.changePenis && Genital_Helper.is_penis(part) && sexPart.GetPartComp().Fluid != null)
                    {
                        StoreAndChangeFluidAmount(part);
                    }
                    else if (Props.changeVagina && Genital_Helper.is_vagina(part) && sexPart.GetPartComp().Fluid != null)
                    {
                        StoreAndChangeFluidAmount(part);
                    }
                    else if (Props.changeBreast && (part.def.defName.ToLower().Contains("breast") || part.def.defName.ToLower().Contains("udder")) && sexPart.GetPartComp().Fluid != null)
                        StoreAndChangeFluidAmount(part);
                    else if (Props.changeOther && sexPart.GetPartComp().Fluid != null)
                    {
                        StoreAndChangeFluidAmount(part);
                    }
                }
            }
            ModLog.Debug($"Changed {storedHediffs.Count()} fluids for {this.parent.pawn}: multiplied by {Props.multiplier}");
        }


        protected void StoreAndChangeFluidAmount(Hediff part)
        {
            // DevNote: Due to the logic before, I assume all of these Casts are save and have values;
            ISexPartHediff sexPart = (ISexPartHediff)part;
            var comp = sexPart.GetPartComp();
            storedHediffs.Add(part);
            comp.partFluidFactor *= Props.multiplier;
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            ModLog.Debug($"Resetting Fluids-Multipliers for {this.parent.pawn} of {storedHediffs.Count()} SexParts");
            for (int i = 0; i < storedHediffs.Count; i++)
            {
                Hediff part = storedHediffs[i];
                if (part is ISexPartHediff sexPart)
                {
                    sexPart.GetPartComp().partFluidFactor *= 1/ Props.multiplier;
                }
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Collections.Look<Hediff>(ref storedHediffs, "storedHediffs", LookMode.Reference);
        }


        public bool WasSuccessfullyApplied() => storedHediffs.Any();
    }
}
