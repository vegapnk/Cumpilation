using RimWorld;
using rjw;
using Verse;

namespace Cumpilation.Reactions
{
    /// <summary>
    /// Handles the updating (and thought-giving) when pawns consume meals with fluid based ingredients. 
    /// 
    /// To work, there need to be two defs: 
    /// - A RecordFluidMappingDef
    /// - The IngestionOutComeDoer
    /// 
    /// Originally, this was from Amaverashi-Sexperience but extended for multiple fluids. 
    /// https://gitgud.io/amevarashi/rjw-sexperience/-/blob/master/Source/RJWSexperience/Cum/IngestionOutcomeDoer_RecordEatenCum.cs?ref_type=heads
    /// </summary>
    public class IngestionOutcomeDoer_RecordEatenFluid : IngestionOutcomeDoer
    {
        public SexFluidDef fluid;
        public float unitAmount = 1.0f;

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
        {
            if (pawn == null || pawn.IsAnimal()) return;
            
            int amount = ingested.stackCount * (int)unitAmount;
            ModLog.Debug($"{pawn} ingested {ingested}, updating records for {fluid} by {amount} ml");
            var Mapping = ReactionUtility.LookupFluid(fluid);

            if (Mapping == null) {
                ModLog.Warning($"Tried to lookup the Mapping for {fluid} in `IngestionOutcomeDoer_RecordEatenFluid`, but failed ");
                return;
            }
            pawn.records.Increment(Mapping.numConsumedRecord);
            pawn.records.AddTo(Mapping.amountConsumedRecord, amount);

            if (Settings.EnableProgressingConsumptionThoughts)
            {
                ReactionUtility.TryGiveThought(Mapping, pawn);
            }
        }

    }
}