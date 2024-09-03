using Cumpilation.Cumflation;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Common
{
    public class HediffComp_FluidChange : HediffComp
    {

        public HediffCompProperties_FluidChange Props => (HediffCompProperties_FluidChange)this.props;


        List<Hediff> changedParts = new List<Hediff>();

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);

            ModLog.Debug($"Running HediffComp_FluidChange PostPostAdd for {this.parent.pawn}");
            Pawn pawn = this.parent.pawn;

            foreach (Hediff part in Genital_Helper.get_AllPartsHediffList(pawn))
            {
                if (part is ISexPartHediff sexPart)
                {
                    if (Props.changePenis && Genital_Helper.is_penis(part) && sexPart.GetPartComp().Fluid != Props.fluid)
                    {
                        sexPart.GetPartComp().Fluid = Props.fluid; 
                        changedParts.Add(part);
                    }
                    else if (Props.changeVagina && Genital_Helper.is_vagina(part) && sexPart.GetPartComp().Fluid != Props.fluid)
                    {
                        sexPart.GetPartComp().Fluid = Props.fluid;
                        changedParts.Add(part);
                    }
                    else if (Props.changeBreast && (part.def.defName.ToLower().Contains("breast") || part.def.defName.ToLower().Contains("udder")) && sexPart.GetPartComp().Fluid != Props.fluid)
                    {
                        sexPart.GetPartComp().Fluid = Props.fluid;
                        changedParts.Add(part);
                    }
                    else if (Props.changeOther && sexPart.GetPartComp().Fluid != Props.fluid)
                    {
                        sexPart.GetPartComp().Fluid = Props.fluid;
                        changedParts.Add(part);
                    }
                }
            }
            ModLog.Debug($"Changed {changedParts.Count()} fluids for {this.parent.pawn}");
        }


        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            ModLog.Debug($"Resetting Fluids for {this.parent.pawn} of {changedParts.Count()} SexParts");
            foreach (Hediff part in changedParts)
            {
                if (part is ISexPartHediff sexPart)
                {
                    sexPart.GetPartComp().Fluid = sexPart.GetPartComp().Def.fluid;
                }
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Collections.Look<Hediff>(ref changedParts, "changedParts");
        }


        public bool WasSuccessfullyApplied() => changedParts.Any();
    }
}
