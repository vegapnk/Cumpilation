using Cumpilation.Cumflation;
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
    public class HediffComp_FluidChange : HediffComp
    {

        public HediffCompProperties_FluidChange Props => (HediffCompProperties_FluidChange)this.props;


        //Dictionary<Hediff,SexFluidDef> changedParts = new Dictionary<Hediff,SexFluidDef>();

        List<Hediff> storedHediffs = new List<Hediff>();
        List<SexFluidDef> storedDefs = new List<SexFluidDef>();

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
                        StoreAndChangeFluid(part);
                    }
                    else if (Props.changeVagina && Genital_Helper.is_vagina(part) && sexPart.GetPartComp().Fluid != Props.fluid)
                    {
                        StoreAndChangeFluid(part);
                    }
                    else if (Props.changeBreast && (part.def.defName.ToLower().Contains("breast") || part.def.defName.ToLower().Contains("udder")) && sexPart.GetPartComp().Fluid != Props.fluid)
                        StoreAndChangeFluid(part);
                    else if (Props.changeOther && sexPart.GetPartComp().Fluid != Props.fluid)
                    {
                        StoreAndChangeFluid(part);
                    }
                }
            }
            ModLog.Debug($"Changed {storedHediffs.Count()} fluids for {this.parent.pawn} to {Props.fluid}");
        }


        protected void StoreAndChangeFluid(Hediff part)
        {
            // DevNote: Due to the logic before, I assume all of these Casts are save and have values;
            ISexPartHediff sexPart = (ISexPartHediff)part;
            var comp = sexPart.GetPartComp();
            storedHediffs.Add(part);
            storedDefs.Add(comp.Fluid);
            comp.Fluid = Props.fluid;
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            ModLog.Debug($"Resetting Fluids for {this.parent.pawn} of {storedDefs.Count()} SexParts");
            for (int i = 0; i < storedHediffs.Count; i++) { 
                Hediff part = storedHediffs[i];
                if (part is ISexPartHediff sexPart)
                {
                    sexPart.GetPartComp().Fluid = storedDefs[i];
                }
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();

            Scribe_Collections.Look<Hediff>(ref storedHediffs, "storedHediffs", LookMode.Reference);
            Scribe_Collections.Look<SexFluidDef>(ref storedDefs, "storedDefs", LookMode.Def);
            /*
             * DevNote: Originally I had a Dictionary, but this was giving me a big big headache with the Scribes. 
             * Someone from the Rimworld Discord tried to help me but I kept getting errors and weird values. 
             * So for now it is two lists, because why not actually. Two lists work quite reliably.
            var workingKeys = new List<Hediff>();
            var workingValues = new List<SexFluidDef>();

            Scribe_Collections.Look<Hediff,SexFluidDef>(
                ref changedParts, "changedParts", keyLookMode:LookMode.Reference, valueLookMode:LookMode.Def,
                keysWorkingList: ref workingKeys, valuesWorkingList: ref workingValues
                ,log
                );
            */
        }


        public bool WasSuccessfullyApplied() => storedHediffs.Any();
    }
}
