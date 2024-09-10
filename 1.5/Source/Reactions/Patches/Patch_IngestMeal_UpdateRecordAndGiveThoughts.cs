using HarmonyLib;
using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Reactions.Patches
{
    /// <summary>
    /// I have one IngestionOutcomeDoer but it's not enough. The individual outcome-doers are not used in meals. 
    /// This triggers all (relevant) ingredients to give their thoughts. 
    /// This also means that if a meal is made of cum and insect jelly, the eater will get two thoughts. 
    /// 
    /// TODO: Right now, the "NumEaten" is updated, but the "AmountEaten" is buggy. 
    /// But as it gives the right thoughts, I am decently happy for now. 
    /// </summary>
    [HarmonyPatch(typeof(ThingComp), nameof(ThingComp.PostIngested))]
    public class Patch_IngestMeal_UpdateRecordAndGiveThoughts
    {
        public static void Postfix(Pawn ingester, ThingComp __instance)
        {
            if (ingester == null || ingester.IsAnimal()) return; 

            if (__instance is CompIngredients meal)
            {
                foreach (var ingredient in meal.ingredients)
                {
                    foreach (IngestionOutcomeDoer_RecordEatenFluid fluidIngestionDoer in ingredient.ingestible.outcomeDoers
                        .Where(doer => doer is IngestionOutcomeDoer_RecordEatenFluid).Cast<IngestionOutcomeDoer_RecordEatenFluid>())
                    {
                        ModLog.Debug($"{ingester} ate {meal.parent} with ingredient {ingredient}");
                        fluidIngestionDoer.DoIngestionOutcome(ingester, meal.parent, 10);
                    }
                }
            }

        }
    }
}
